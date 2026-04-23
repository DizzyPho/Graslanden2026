using GraslandenBL.Builders;
using GraslandenBL.Domain;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using GraslandenLevenshtein;
using GraslandenValidation;

namespace GraslandenDL.FileReaders
{
    public class FileReaderTXT : IFileReader
    {
        private string _indicatorValuesPath;


        public FileReaderTXT(string indicatorValuesPath)
        {
            _indicatorValuesPath = indicatorValuesPath;
        }

        public List<Measurement> ReadFile(string inventoryPath, out Dictionary<string, MessageType> messages)
        {
            List<Measurement> results = new List<Measurement>();
            Dictionary<string, Species> tylerSpeciesList = new Dictionary<string, Species>();
            messages = new Dictionary<string, MessageType>();
            Dictionary<string, int> unfoundNames = new Dictionary<string, int>();

            #region TylerDatabase
            using (StreamReader streamReader = new StreamReader(_indicatorValuesPath))
            {
                // Skip column names
                streamReader.ReadLine();
                int currentLine = 2;

                while (!streamReader.EndOfStream)
                {
                    string[] lineSections = streamReader.ReadLine().Split('|');

                    // Make object through builder
                    // Builder always returns list of error messages in case of missing/wrong values
                    SpeciesBuilder speciesBuilder = new SpeciesBuilder(name: lineSections[0], fileName: "TYLER");
                    speciesBuilder.AddMoisture(moistureString: lineSections[15]);
                    speciesBuilder.AddPh(phString: lineSections[16]);
                    speciesBuilder.AddNitrogen(nitrogenString: lineSections[17]);
                    speciesBuilder.AddNectarValue(nectarValueString: lineSections[9]);
                    speciesBuilder.AddBiodiversity(biodiversityString: lineSections[8]);
                    Species species = speciesBuilder.Build();

                    // Object is only null if name is empty/null
                    if (species != null)
                    {
                        tylerSpeciesList.Add(key: species.Name,
                                                value: species);
                    }

                    currentLine++;
                }
            }
            #endregion

            #region InventoryDatabase
            List<string> firstLineCurrentCampus = new List<string>();

            // Name of current tab's campus
            string currentCampus = "";
            int currentLineWithinCampus = 0;

            // Plots and their respective column indices
            Dictionary<string, Plot> plots = new Dictionary<string, Plot>();

            // This list is used to check for similar plant names
            HashSet<string> allSpeciesNames = new HashSet<string>();

            using (StreamReader streamReader = new StreamReader(inventoryPath))
            {
                int currentLine = 1;

                // Keep plot properties in separate lists to combine later
                List<string> plotNames = new List<string>();
                List<string> plotAreas = new List<string>();
                List<string> plotManagementTypes = new List<string>();

                List<Measurement> measurements = new List<Measurement>();

                // Keep how many species every plot has
                // This will be used to get values at bottom of every plot list
                Dictionary<string, int> speciesPerPlot = new Dictionary<string, int>();

                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    bool wasPlotFound = true;

                    // Extract campus name from tab title
                    if (line.Contains("Worksheet"))
                    {
                        currentCampus = line.Split(':')[1].Replace("---", "").Trim();
                        currentLineWithinCampus = 0;
                        plotNames = new List<string>();
                        plotAreas = new List<string>();
                        plotManagementTypes = new List<string>();
                        wasPlotFound = true;
                    }
                    else
                    {
                        string[] lineSections = line.Split('|');

                        // Save line that contains plot codes
                        if (currentLineWithinCampus == 0) firstLineCurrentCampus = lineSections.ToList();

                        // These rows contain general plot info
                        if (lineSections[0].Trim() != string.Empty)
                        {
                            // Only the first three lines are relevant
                            if (currentLineWithinCampus <= 2)
                            {
                                for (int i = 1; i < lineSections.Length; i++)
                                {
                                    string currentCell = lineSections[i];

                                    if (currentCell.Trim() != string.Empty)
                                    {
                                        // Plot codes
                                        if (currentLineWithinCampus == 0)
                                        {
                                            string campusCode = "";
                                            foreach (char character in currentCell)
                                            {
                                                if (!char.IsDigit(character)) campusCode += character;
                                            }

                                            // Only save actual plot codes
                                            if (campusCode.Length > 0
                                                && campusCode.Length <= currentCampus.Length
                                                    && currentCell.StartsWith(currentCampus.Substring(startIndex: 0, length: campusCode.Length)
                                                                                           .ToUpper()))
                                            {
                                                plotNames.Add(currentCell);
                                            }
                                            else
                                            {
                                                // Check for typos
                                                if (campusCode.Length > 0
                                                    && campusCode.Length <= currentCampus.Length
                                                    && Levenshtein.Distance(value1: campusCode,
                                                                            value2: currentCampus.Substring(startIndex: 0, length: campusCode.Length)
                                                                                                 .ToUpper()) < 3)
                                                {
                                                    messages.TryAdd(key: $"{currentCampus} {currentCell} | Ongeldige plotcode", value: MessageType.Error);
                                                }
                                            }
                                        }

                                        // Areas (throw away data past plot info table)
                                        else if (currentLineWithinCampus == 1
                                                 && plotAreas.Count < plotNames.Count)
                                        {
                                            plotAreas.Add(currentCell);
                                        }

                                        // Management types (throw away data past plot info table)
                                        else if (currentLineWithinCampus == 2
                                                 && plotManagementTypes.Count < plotNames.Count)
                                        {
                                            plotManagementTypes.Add(currentCell);
                                        }
                                    }
                                }
                                if (plotNames.Count == 0)
                                {
                                    wasPlotFound = false;
                                }
                            }

                            // Combine data once every list has been filled
                            else if (currentLineWithinCampus == 3 && wasPlotFound == true)
                            {
                                for (int i = 0; i < plotNames.Count; i++)
                                {

                                    // Is the data valid? If false, return error messages
                                    if (PlotValidation.Validate(code: plotNames[i],
                                                                areaString: plotAreas[i],
                                                                campus: currentCampus,
                                                                managementTypeString: plotManagementTypes[i],
                                                                out Dictionary<string, MessageType> plotMessages))
                                    {
                                        // Default value that gets overwritten
                                        ManagementType managementType = ManagementType.Intensief;

                                        // Exception will not get thrown since value has already been validated
                                        managementType = plotManagementTypes[i].Trim().ToUpper() switch
                                        {
                                            "INTENSIEF" => ManagementType.Intensief,
                                            "EXTENSIEF" => ManagementType.Extensief,
                                            "NETHEIDSBOORD" => ManagementType.Netheidsboord,
                                            "SCHAPENWEIDE" => ManagementType.Schapenweide,
                                            _ => throw new Exception("Managementtype niet gevonden")
                                        };

                                        // Plot type gets filled in later
                                        Plot newPlot = new Plot(code: plotNames[i],
                                                                areaSqMeters: double.Parse(plotAreas[i]),
                                                                campus: currentCampus,
                                                                managementType: managementType,
                                                                plotType: "");

                                        foreach (KeyValuePair<string, MessageType> remark in plotMessages)
                                        {
                                            newPlot.Errors.Add(key: $"{currentCampus} {plotNames[i]} | {remark.Key}", value: remark.Value);
                                        }

                                        plots.Add(key: plotNames[i],
                                                  value: newPlot);
                                    }
                                    else
                                    {
                                        // Collect every error location & message in a collective list
                                        foreach (KeyValuePair<string, MessageType> error in plotMessages)
                                        {
                                            messages.Add(key: $"{currentCampus} {plotNames[i]} | {error.Key}", value: MessageType.Error);
                                        }
                                    }
                                }
                            }
                        }

                        // Every line past the plot info table
                        else if (wasPlotFound == true)
                        {
                            for (int i = 0; i < lineSections.Length; i++)
                            {
                                if (lineSections[i] == "Potentilla indica")
                                    Console.WriteLine();
                                // Column == column of a plant name (=> there are 5 cells in between)
                                if ((i - 1) % 6 == 0)
                                {
                                    // Plant name isn't empty
                                    if (lineSections[i].Trim() != "" && lineSections[i + 1].Trim() != "")
                                    {
                                        // Retrieve plot from plot code in same column
                                        if (plots.TryGetValue(key: firstLineCurrentCampus[i],
                                                              out Plot currentPlot))
                                        {

                                            // Make object through builder
                                            // Builder always returns list of error messages in case of missing/wrong values
                                            SpeciesBuilder speciesBuilder = new SpeciesBuilder(name: lineSections[i], fileName: "INV");
                                            Species species;

                                            // Names in Tyler database include extra info we don't want at the end
                                            List<string> tylerNameResults = tylerSpeciesList.Keys.Where(s => s.StartsWith(lineSections[i]))
                                                                                                 .ToList();
                                            // Check if a match was found
                                            if (tylerNameResults.Count > 0)
                                            {
                                                // There can only be 1 match
                                                string tylerSpeciesName = tylerNameResults.First();

                                                Species tylerSpecies = tylerSpeciesList[tylerSpeciesName];

                                                // Replace empty values with inventory values (Add method returns bool if value couldn't be added)
                                                if (!speciesBuilder.AddMoisture(moistureString: tylerSpecies.Moisture.ToString()))
                                                {
                                                    speciesBuilder.AddMoisture(moistureString: lineSections[i + 2]);
                                                }

                                                if (!speciesBuilder.AddPh(phString: tylerSpecies.Ph.ToString()))
                                                {
                                                    speciesBuilder.AddPh(phString: lineSections[i + 3]);
                                                }

                                                if (!speciesBuilder.AddNitrogen(nitrogenString: tylerSpecies.Nitrogen.ToString()))
                                                {
                                                    speciesBuilder.AddNitrogen(nitrogenString: lineSections[i + 4]);
                                                }

                                                speciesBuilder.AddNectarValue(nectarValueString: tylerSpecies.Nectarvalue.ToString());
                                                speciesBuilder.AddBiodiversity(biodiversityString: tylerSpecies.Biodiversity.ToString());
                                                speciesBuilder.AddRating(ratingString: lineSections[i + 5]);

                                                species = speciesBuilder.Build();
                                            }
                                            else
                                            {
                                                speciesBuilder.AddMoisture(moistureString: lineSections[i + 2]);
                                                speciesBuilder.AddPh(phString: lineSections[i + 3]);
                                                speciesBuilder.AddNitrogen(nitrogenString: lineSections[i + 4]);
                                                speciesBuilder.AddRating(ratingString: lineSections[i + 5]);

                                                species = speciesBuilder.Build();

                                                // If the species was not found in the Tyler database, we use the inventory values and add it to the unfound names counter
                                                if (!unfoundNames.ContainsKey(key: lineSections[i]))
                                                {
                                                    unfoundNames.Add(key: lineSections[i],
                                                                        value: 1);
                                                }
                                                else unfoundNames[lineSections[i]]++;
                                            }

                                            // Is the data valid? If false, return error messages
                                            if (MeasurementValidation.Validate(coverage: lineSections[i + 1],
                                                                                out Dictionary<string, MessageType> measurementMessages))
                                            {
                                                Measurement newMeasurement = new Measurement(species: species,
                                                                                                coverage: lineSections[i + 1],
                                                                                                plot: currentPlot);

                                                // Check for spelling mistakes
                                                foreach (string speciesName in allSpeciesNames)
                                                {
                                                    // Amount of differing characters + 1
                                                    int distance = Levenshtein.Distance(value1: newMeasurement.Species.Name.Trim(),
                                                                                        value2: speciesName.Trim());

                                                    // 1 == equal, 3 == 2 character difference
                                                    if (distance < 3 && distance > 1)
                                                    {
                                                        // Remarks need to be unique
                                                        if (!messages.ContainsKey(key: $"{speciesName.Trim()} | {newMeasurement.Species.Name.Trim()} Zijn dit twee verschillende planten?"))
                                                        {
                                                            messages.TryAdd(key: $"{newMeasurement.Species.Name.Trim()} | {speciesName.Trim()} Zijn dit twee verschillende planten?", value: MessageType.Remark);
                                                        }
                                                    }
                                                }

                                                foreach (KeyValuePair<string, MessageType> remark in measurementMessages)
                                                {
                                                    newMeasurement.Errors.Add(key: $"INV{currentLine} | {remark.Key}",
                                                                                value: remark.Value);
                                                }

                                                results.Add(newMeasurement);
                                                allSpeciesNames.Add(species.Name);

                                            }
                                            else
                                            {
                                                foreach (KeyValuePair<string, MessageType> error in measurementMessages)
                                                {
                                                    messages.Add(key: $"INV{currentLine} | {error.Key}",
                                                                    value: error.Value);
                                                }
                                            }

                                            // Add and increment species count of plot
                                            if (currentPlot != null)
                                            {
                                                if (!speciesPerPlot.ContainsKey(currentPlot.Code))
                                                {
                                                    speciesPerPlot.Add(key: currentPlot.Code,
                                                                        value: 1);
                                                }
                                                else speciesPerPlot[currentPlot.Code]++;
                                            }

                                        }
                                    }

                                    // Every line with general data info at bottom of a plot's species list
                                    else if (lineSections.Count() > i + 2 && lineSections[i + 2].Trim() != "")
                                    {
                                        // Find plot code and add plot type
                                        if (plots.TryGetValue(key: firstLineCurrentCampus[i],
                                                                out Plot plot))
                                        {
                                            if (speciesPerPlot[plot.Code] == currentLineWithinCampus - 15)
                                            {
                                                plot.PlotType = lineSections[i + 2];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        currentLineWithinCampus++;
                    }
                    currentLine++;
                }
            }
            #endregion

            #region General remarks
            // General remarks for species that weren't found in Tyler database & how many times they appear
            foreach (KeyValuePair<string, int> name in unfoundNames)
            {
                if (name.Value > 1)
                    messages.Add($"{name.Key} werd niet gevonden in de Tylerdatabank. Inventarisatiewaarden werden gebruikt voor {name.Value} metingen.", MessageType.Remark);
                else
                    messages.Add($"{name.Key} werd niet gevonden in de Tylerdatabank. Inventarisatiewaarden werden gebruikt voor {name.Value} meting.", MessageType.Remark);
            }
            #endregion

            if (results.Count > 0)
            {
                return results;
            }
            else
            {
                throw new Exception("Er werd geen enkel plot gevonden. Was dit het juiste bestand?");
            }
        }
    }
}