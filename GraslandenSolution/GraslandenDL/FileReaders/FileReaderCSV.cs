using GraslandenBL.Domain;
using GraslandenBL.FactoryResults;
using GraslandenBL.Interfaces;
using GraslandenDL.Factories;

namespace GraslandenDL.FileReaders
{
    public class FileReaderCSV : IFileReader
    {
        private string _inventoryPath;
        private string _indicatorValuesPath;

        public FileReaderCSV(string inventoryPath, string indicatorValuesPath)
        {
            _inventoryPath = inventoryPath;
            _indicatorValuesPath = indicatorValuesPath;
        }
        
        public List<FactoryResult> ReadFile()
        {
            Dictionary<string, Species> speciesList = new Dictionary<string, Species>();

            using (StreamReader streamReader = new StreamReader(_indicatorValuesPath))
            {
                while(!streamReader.EndOfStream)
                {
                    string[] line = streamReader.ReadLine().Split(';');
                    FactoryResult<Species> newSpecies = SpeciesFactory.CreateSpecies();
                    if (newSpecies.IsSuccess) speciesList.Add(newSpecies.Result.Name, newSpecies.Result);
                }
            }
        }
    }
}
