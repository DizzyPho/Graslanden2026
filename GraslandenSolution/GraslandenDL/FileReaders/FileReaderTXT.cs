using GraslandenBL.Builders;
using GraslandenBL.Domain;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using GraslandenBL.Results;
using GraslandenLevenshtein;
using GraslandenValidation;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    SpeciesBuilder speciesBuilder = new SpeciesBuilder(name: lineSections[0])
                                                    .AddMoisture(moistureString: lineSections[15])
                                                    .AddPh(phString: lineSections[16])
                                                    .AddNitrogen(nitrogenString: lineSections[17])
                                                    .AddNectarValue(nectarValueString: lineSections[9])
                                                    .AddBiodiversity(biodiversityString: lineSections[8]);
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
         
            using(StreamReader streamReader = new StreamReader(inventoryPath))
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
                
                while(!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();

                    // Extract campus name from tab title
                    if (line.Contains("Worksheet"))
                    {
                        currentCampus = line.Split(':')[1].Replace("---", "").Trim();
                        currentLineWithinCampus = 0;
                        plotNames = new List<string>();
                        plotAreas = new List<string>();
                        plotManagementTypes = new List<string>();
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
                                        if(currentLineWithinCampus == 0)
                                        {
                                            // Only save actual plot codes
                                            if (currentCell.StartsWith(currentCampus.Substring(startIndex: 0, length: 3)
                                                                                    .ToUpper()))
                                            {
                                                plotNames.Add(currentCell);
                                            }
                                        }

                                        // Areas (throw away data past plot info table)
                                        else if(currentLineWithinCampus == 1 && plotAreas.Count < plotNames.Count)
                                        {
                                            plotAreas.Add(currentCell);
                                        }

                                        // Management types (throw away data past plot info table)
                                        else if (currentLineWithinCampus == 2 && plotManagementTypes.Count < plotNames.Count)
                                        {
                                            plotManagementTypes.Add(currentCell);
                                        }
                                    }
                                }
                            }
                            // Combine data once every list has been filled
                            else if (currentLineWithinCampus == 3)
                            {
                                for (int i = 0; i < plotNames.Count; i++)
                                {

                                    // Is the data valid? If false, return error messages
                                    if (PlotValidation.Validate(code: plotNames[i],
                                        areaString: plotAreas[i],
                                        campus: currentCampus,
                                        managementTypeString: plotManagementTypes[i],
                                        out List<string> plotErrors))
                                    {
                                        // Default value that gets overwritten
                                        ManagementType managementType = ManagementType.Intensief;

                                        managementType = plotManagementTypes[i].Trim().ToUpper() switch
                                        {
                                            "INTENSIEF" => ManagementType.Intensief,
                                            "EXTENSIEF" => ManagementType.Extensief,
                                            "NETHEIDSBOORD" => ManagementType.Netheidsboord,
                                            "SCHAPENWEIDE" => ManagementType.Schapenweide,
                                        };

                                        // Plot type gets filled in later
                                        plots.Add(key: plotNames[i],
                                                  value: new Plot(code: plotNames[i],
                                                                  areaSqMeters: double.Parse(plotAreas[i]),
                                                                  campus: currentCampus,
                                                                  managementType: managementType,
                                                                  plotType: ""));
                                    }
                                    else
                                    {
                                        // Collect every error location & message in a collective list
                                        foreach (string error in plotErrors)
                                        {
                                            messages.Add($"$INVENTORY{currentLine} | {error}", MessageType.Error);
                                        }
                                    }
                                }
                            }
                        }

                        // Every line past the plot info table
                        else
                        {
                            for (int i = 0; i < lineSections.Length; i++)
                            {
                                // Column == column of a plant name (=> there are 5 cells in between)
                                if ((i - 1) % 6 == 0)
                                {
                                    // Plant name isn't empty
                                    if (lineSections[i].Trim() != "" && lineSections[i + 1].Trim() != "")
                                    {
                                        // Retrieve plot from plot code in same column
                                        Plot currentPlot = plots[firstLineCurrentCampus[i]];

                                        // Make object through builder
                                        // Builder always returns list of error messages in case of missing/wrong values
                                        SpeciesBuilder speciesBuilder = new SpeciesBuilder(lineSections[i])
                                            .AddMoisture(lineSections[i + 2])
                                            .AddPh(lineSections[i + 3])
                                            .AddNitrogen(lineSections[i + 4])
                                            .AddRating(lineSections[i + 5]);

                                        Species species = speciesBuilder.Build();
                    
                                        // Is the data valid? If false, return error messages
                                        if (MeasurementValidation.Validate(lineSections[i + 1], out List<string> measurementErrors))
                                        {
                                            results.Add(new Measurement(species, lineSections[i + 1], currentPlot));
                                            allSpeciesNames.Add(species.Name);
                                        }
                                        else
                                        {
                                            foreach(string error in measurementErrors)
                                            {
                                                messages.Add($"$INVENTORY{currentLine} | {error}", MessageType.Error);
                                            }
                                        }

                                        // Add and increment species count of plot
                                        if (!speciesPerPlot.ContainsKey(currentPlot.Code))
                                        {
                                            speciesPerPlot.Add(currentPlot.Code, 1);
                                        }
                                        else speciesPerPlot[currentPlot.Code]++;
                                    }
                                    
                                    // Every line with general data info at bottom of a plot's species list
                                    else if (lineSections.Count() > i + 2 && lineSections[i + 2].Trim() != "")
                                    {
                                        // Find plot code and add plot type
                                        if(plots.TryGetValue(firstLineCurrentCampus[i], out Plot currentPlot))
                                        {
                                            if (speciesPerPlot[currentPlot.Code] == currentLineWithinCampus - 15)
                                            {
                                                currentPlot.PlotType = lineSections[i + 2];
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

            #region SpeciesMatching
            foreach (Measurement inventoryMeasurement in results)
            {
                // Check for spelling mistakes
                foreach(string speciesName in allSpeciesNames)
                {
                    // Amount of differing characters + 1
                    int distance = Levenshtein.Distance(inventoryMeasurement.Species.Name.Trim(), speciesName.Trim());

                    // 1 == equal, 3 == 2 character difference
                    if (distance < 3 && distance > 1)
                    {
                        // Remarks need to be unique
                        if (!messages.ContainsKey($"{speciesName.Trim()} | {inventoryMeasurement.Species.Name.Trim()} Zijn dit twee verschillende planten?"))
                        {
                            messages.TryAdd($"{inventoryMeasurement.Species.Name.Trim()} | {speciesName.Trim()} Zijn dit twee verschillende planten?", MessageType.Remark);
                        }
                    }
                }

                // Names in Tyler database include extra info we don't want at the end
                List<string> tylerNameResults = tylerSpeciesList.Keys.Where(s => s.StartsWith(inventoryMeasurement.Species.Name)).ToList();

                // Check if a match was found
                if (tylerNameResults.Count > 0)
                {
                    // There can only be 1 match
                    string tylerSpeciesName = tylerNameResults.First();

                    Species tylerSpecies = tylerSpeciesList[tylerSpeciesName];

                    // Replace empty values with inventory values
                    if (tylerSpecies.Moisture != null)
                    {
                        inventoryMeasurement.Species.Moisture = tylerSpecies.Moisture;
                    }
                    else
                    {
                        if (inventoryMeasurement.Species.Moisture != null)
                        {
                            inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Vochtgehalte ontbreekt in Tylerdatabank; inventarisatiewaarde werd gebruikt.", MessageType.Remark);
                        }
                        else
                        {
                            inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Vochtgehalte ontbreekt in beide tabellen.", MessageType.Error);
                        }
                    }

                    if (tylerSpecies.Ph != null)
                    {
                        inventoryMeasurement.Species.Ph = tylerSpecies.Ph;
                    }
                    else
                    {
                        if (inventoryMeasurement.Species.Ph != null)
                        {
                            inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Zuurtegraad ontbreekt in Tylerdatabank; inventarisatiewaarde werd gebruikt.", MessageType.Remark);
                        }
                        else
                        {
                            inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Zuurtegraad ontbreekt in beide tabellen.", MessageType.Error);
                        }
                    }

                    if (tylerSpecies.Nitrogen != null)
                    {
                        inventoryMeasurement.Species.Nitrogen = tylerSpecies.Nitrogen;
                    }
                    else
                    {
                        if (inventoryMeasurement.Species.Nitrogen != null)
                        {
                            inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Stikstofgehalte ontbreekt in Tylerdatabank; inventarisatiewaarde werd gebruikt.", MessageType.Remark);
                        }
                        else
                        {
                            inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Stikstofgehalte ontbreekt in beide tabellen.", MessageType.Error);
                        }
                    }

                    // These aren't included in inventory file
                    if (tylerSpecies.Nectarvalue != null)
                    {
                        inventoryMeasurement.Species.Nectarvalue = tylerSpecies.Nectarvalue;

                        if (tylerSpecies.Nectarvalue < 0) inventoryMeasurement.Species.Errors.Add($"{inventoryMeasurement.Species.Name} | Foute nectarwaarde: '{inventoryMeasurement.Species.Nectarvalue}' moet een positief getal zijn.", MessageType.Error);
                    }
                    else
                    {
                        inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Nectarwaarde niet gevonden.", MessageType.Remark);
                    }
                    if (tylerSpecies.Biodiversity != null)
                    {
                        inventoryMeasurement.Species.Biodiversity = tylerSpecies.Biodiversity;

                        if(tylerSpecies.Biodiversity < 0) inventoryMeasurement.Species.Errors.Add($"{inventoryMeasurement.Species.Name} | Foute biodiversiteit: '{inventoryMeasurement.Species.Biodiversity}' moet een positief getal zijn.", MessageType.Error);
                    }
                    else
                    {
                        inventoryMeasurement.Errors.Add($"{tylerSpeciesName} | Biodiversiteit niet gevonden.", MessageType.Remark);
                    }
                }
                else
                {
                    // If the species was not found in the Tyler database, we use the inventory values and add it to the unfound names counter
                    if (!unfoundNames.ContainsKey(inventoryMeasurement.Species.Name))
                    {
                        unfoundNames.Add(inventoryMeasurement.Species.Name, 1);
                    }
                    else unfoundNames[inventoryMeasurement.Species.Name]++;
                }
            }
            #endregion

            #region General remarks
            // General remarks for species that weren't found in Tyler database & how many times they appear
            foreach (KeyValuePair<string, int> name in unfoundNames)
            {
                if(name.Value > 1)
                    messages.Add($"{name.Key} werd niet gevonden in de Tylerdatabank. Inventarisatiewaarden werden gebruikt voor {name.Value} metingen.", MessageType.Remark);
                else
                messages.Add($"{name.Key} werd niet gevonden in de Tylerdatabank. Inventarisatiewaarden werden gebruikt voor {name.Value} meting.", MessageType.Remark);
            }
            #endregion

            return results;
        }
    }
}