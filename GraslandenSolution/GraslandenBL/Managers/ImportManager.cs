using GraslandenBL.Domain;
using GraslandenBL.Interfaces;

namespace GraslandenBL.Managers
{
    public class ImportManager
    {
        private IRepository _repository;
        private IFileReader _fileReader;

        public ImportManager(IRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
        }

        public void ImportData(string inventoryPath)
        {
            Inventory i = new Inventory(DateTime.UtcNow,"Tester");
            List<Measurement> measurements = _fileReader.ReadFile(inventoryPath);
            i.Measurements = measurements;
            _repository.ImportInventory(i);

        }
    }
}
