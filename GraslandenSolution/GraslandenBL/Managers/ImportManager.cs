using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;
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
        public InventoryDTO ImportData(string inventoryPath, InventoryDTO inventoryDTO)
        {
            Inventory inventory = new Inventory(inventoryDTO.Date, inventoryDTO.Name);
            List<Measurement> measurements = _fileReader.ReadFile(inventoryPath, out Dictionary<string, MessageType> messages);
            if(messages.Count > 0)
            {
                // _repository.ImportMessages(messages);
            }
            inventory.Measurements = measurements;
            int inventoryId = _repository.ImportInventory(inventory);
            inventoryDTO.Id = inventoryId;

            return inventoryDTO;
        }
    }
}
