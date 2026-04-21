using GraslandenBL.Domain;
using GraslandenBL.Enums;
using GraslandenBL.Interfaces;
using GraslandenValidation;

namespace GraslandenDL.FileReaders
{
    public class FileReaderTXT : IFileReader
    {
        private string _indicatorValuesPath;
        private List<string> _errorMessages = new List<string>();

        public FileReaderTXT(string indicatorValuesPath)
        {
            _indicatorValuesPath = indicatorValuesPath;
        }
        
        public List<Measurement> ReadFile(string inventoryPath)
        {
            List<Measurement> results = new List<Measurement>();
            Dictionary<string, Species> tylerSpeciesList = new Dictionary<string, Species>();

            using (StreamReader streamReader = new StreamReader(_indicatorValuesPath))
            {
                // Skip column names
                streamReader.ReadLine();
                while (!streamReader.EndOfStream)
                {
                    string[] lineSections = streamReader.ReadLine().Split('|');

                    if (SpeciesValidation.Validate(name: lineSections[0],
                                                moistureString: lineSections[15],
                                                phString: lineSections[16],
                                                nitrogenString: lineSections[17],
                                                nectarValueString: lineSections[9],
                                                biodiversityString: lineSections[8],
                                                ratingString: null, out List<string> errorMessages))
                    {
                        Species newSpecies = new Species(id: null,
                                                        name: lineSections[0],
                                                        moisture: int.Parse(lineSections[15]),
                                                        ph: int.Parse(lineSections[16]),
                                                        nitrogen: int.Parse(lineSections[17]),
                                                        nectarvalue: int.Parse(lineSections[9]),
                                                        biodiversity: int.Parse(lineSections[8]),
                                                        rating: null);

                        tylerSpeciesList.Add(newSpecies.Name, newSpecies);
                    }
                    else
                    {
                        _errorMessages.AddRange(errorMessages);
                    }
                }
            }

            List<string> currentCampusLineSections = new List<string>();
            string currentCampus = "";
            int currentLineWithinCampus = 0;

            // Plots and their respective column indices
            Dictionary<string, Plot> plots = new Dictionary<string, Plot>();
            // List<Species> inventorySpeciesList = new List<Species>();

            // TO DO: replace path with _inventoryPath
            using(StreamReader streamReader = new StreamReader(inventoryPath))
            {
                List<string> plotNames = new List<string>();
                List<double> plotAreas = new List<double>();
                List<string> plotManagementTypes = new List<string>();
                List<Measurement> measurements = new List<Measurement>();

                while(!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Contains("Worksheet"))
                    {
                        currentCampus = line.Split(':')[1].Replace("---", "").Trim();
                        currentLineWithinCampus = 0;
                        plotNames = new List<string>();
                        plotAreas = new List<double>();
                        plotManagementTypes = new List<string>();
                    }
                    else
                    {
                        string[] lineSections = line.Split('|');

                        if (currentLineWithinCampus == 0) currentCampusLineSections = lineSections.ToList();

                        if (lineSections[0].Trim() != string.Empty)
                        {
                            if (currentLineWithinCampus <= 2)
                            {
                                for (int i = 1; i < lineSections.Length; i++)
                                {
                                    string currentCell = lineSections[i];
                                    if (currentCell.Trim() != string.Empty)
                                    {
                                        if(currentLineWithinCampus == 0)
                                        {
                                            if (currentCell.StartsWith(currentCampus.Substring(0,3).ToUpper()))
                                            {
                                                plotNames.Add(currentCell);
                                            }
                                        }
                                        else if(currentLineWithinCampus == 1 && plotAreas.Count < plotNames.Count)
                                        {
                                            try
                                            {
                                                plotAreas.Add(double.Parse(currentCell));
                                            }
                                            catch
                                            {
                                                plotAreas.Add(-1);
                                            }
                                        }
                                        else if(currentLineWithinCampus == 2 && plotManagementTypes.Count < plotManagementTypes.Count)
                                        {
                                            plotManagementTypes.Add(currentCell);
                                        }
                                    }
                                }
                            }
                            else if (currentLineWithinCampus == 3)
                            {
                                for (int i = 0; i < plotNames.Count; i++)
                                {
                                    ManagementType managementType = ManagementType.Intensief;
                                    try
                                    {
                                        managementType = plotManagementTypes[i].Trim().ToUpper() switch
                                        {
                                            "INTENSIEF" => ManagementType.Intensief,
                                            "EXTENSIEF" => ManagementType.Extensief,
                                            "NETHEIDSBOORD" => ManagementType.Netheidsboord,
                                            "SCHAPENWEIDE" => ManagementType.Schapenweide,
                                        };
                                    }
                                    catch
                                    {
                                    }
                                    plots.Add(plotNames[i], new Plot(plotNames[i], plotAreas[i], currentCampus, managementType, new PlotType("test", "test")));
                                }
                            }
                        }

                        if (currentLineWithinCampus > 10)
                        {
                            for (int i = 0; i < lineSections.Length; i++)
                            {
                                if ((i - 1) % 6 == 0 && lineSections[i].Trim() != "")
                                {
                                    if (lineSections[i + 1].Trim() != "")
                                    {
                                        try
                                        {
                                            Rating rating = lineSections[i + 5].Trim() switch
                                            {
                                                "+++" => Rating.Sleutel,
                                                "++" => Rating.Begeleidend,
                                                "+" => Rating.Algemeen,
                                                "0" => Rating.Ruderaal,
                                                "-" => Rating.Invasief
                                            };
                                            Plot currentPlot = plots[currentCampusLineSections[i]];


                                            Species newSpecies = new Species(id: null,
                                                                    name: lineSections[i].Trim(),
                                                                    moisture: int.Parse(lineSections[i + 2]),
                                                                    ph: int.Parse(lineSections[i + 3]),
                                                                    nitrogen: int.Parse(lineSections[i + 4]),
                                                                    nectarvalue: null,
                                                                    biodiversity: null,
                                                                    rating: rating);

                                            measurements.Add(new Measurement(newSpecies, lineSections[i + 1], currentPlot));
                                        }
                                        catch
                                        {
                                            _errorMessages.Add($"{currentCampus}");
                                        }
                                    }
                                }
                            }
                        }
                        currentLineWithinCampus++;
                    }
                }
            }

            // Check gras.txt list to see if species are found in tyler database, then get the values. If values are missing, use own values.
            foreach(Measurement inventoryMeasurement in results)
            {
                if(tylerSpeciesList.TryGetValue(inventoryMeasurement.Species.Name, out Species tylerSpecies))
                {
                    if (tylerSpecies.Moisture != null) inventoryMeasurement.Species.Moisture = tylerSpecies.Moisture;
                    if (tylerSpecies.Ph != null) inventoryMeasurement.Species.Ph = tylerSpecies.Ph;
                    if (tylerSpecies.Nitrogen != null) inventoryMeasurement.Species.Nitrogen = tylerSpecies.Nitrogen;
                    if (tylerSpecies.Nectarvalue != null) inventoryMeasurement.Species.Nectarvalue = tylerSpecies.Nectarvalue;
                    if (tylerSpecies.Biodiversity != null) inventoryMeasurement.Species.Biodiversity = tylerSpecies.Biodiversity;
                }
            }
            // gh - hp stuff
            return results;
        }
    }
}