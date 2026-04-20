using GraslandenBL.Domain;
using GraslandenBL.Interfaces;

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

        public List<Measurement> ImportInventory()
        {
            throw new NotImplementedException();
        }
    }
}
