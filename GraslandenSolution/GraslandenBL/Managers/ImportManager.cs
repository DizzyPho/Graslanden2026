using GraslandenBL.Domain;
using GraslandenBL.DTOs;
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

        // returns ID of inventory in DB
        public InventoryDTO ImportData(string inventoryPath, DateTime inventoryDate, string inventoryName)
        {
            Inventory inventory = new Inventory(inventoryDate, inventoryName);
            List<Measurement> measurements = _fileReader.ReadFile(inventoryPath);
            inventory.Measurements = measurements;
            int inventoryId = _repository.ImportInventory(inventory);

            return new InventoryDTO(inventoryId, inventoryDate, inventoryName);
        }
    }
}
