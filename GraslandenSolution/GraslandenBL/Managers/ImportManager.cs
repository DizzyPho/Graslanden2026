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
        private List<Measurement> _measurements;
        private Dictionary<string, MessageType> _messages;

        public ImportManager(IRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
            _measurements = new List<Measurement>();
            _messages = new Dictionary<string, MessageType>();
        }

        public bool ReadFile(string inventoryPath)
        {
            try
            {
                _measurements = _fileReader.ReadFile(inventoryPath, out Dictionary<string, MessageType> incomingMessages);
                _messages = incomingMessages;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        // returns ID of inventory in DB
        public InventoryDTO ImportData(string inventoryPath, InventoryDTO inventoryDTO)
        {
            if (_measurements.Count > 0)
            {
                Inventory inventory = new Inventory(inventoryDTO.Date, inventoryDTO.Name);
                inventory.Measurements = _measurements;
                int inventoryId = _repository.ImportInventory(inventory);
                inventoryDTO.Id = inventoryId;

                if (_messages.Count > 0)
                {
                    _repository.InsertMessages(inventoryId, _messages);
                }

                return inventoryDTO;
            }
            else return null;
        }
    }
}
