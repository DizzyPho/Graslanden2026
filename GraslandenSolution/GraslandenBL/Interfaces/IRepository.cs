using GraslandenBL.Domain;
using GraslandenBL.DTOs;
using GraslandenBL.Enums;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public int ImportInventory(Inventory inventory);

        public List<Species> GetAllSpecies();

        public List<CampusDTO> GetAllCampusesDTO(int inventoryID);

        public List<InventoryDTO> GetInventoryDTOs();

        public int ImportEmptyInventory(InventoryDTO inventoryDTO);

        public MeasurementDTO InsertMeasurement(string plotCode, string species, string coverage, int inventoryId);

        public List<MeasurementDTO> GetMeasurementsDTOForPlot(int inventoryID, string code);

        public void InsertSpecies(Species species);

        public bool DeleteInventory(int inventoryId);

        public CampusDTO GetCampusDTO(int inventoryID,string campus);

        public void InsertMessages(int inventoryID, Dictionary<string, MessageType> messages);

        public void DeleteMeasurement(int measurementDTO_id);

        public Dictionary<string, List<MessageDTO>> GetAllMessages();
    }
}