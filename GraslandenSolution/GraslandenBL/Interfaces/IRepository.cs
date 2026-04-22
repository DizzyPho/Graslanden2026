using GraslandenBL.Domain;
using GraslandenBL.DTOs;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public int ImportInventory(Inventory inventory);

        public List<Species> GetAllSpecies();

        public HashSet<string> GetAllCampuses();

        public List<InventoryDTO> GetInventoryDTOs();
        public int ImportEmptyInventory(InventoryDTO inventoryDTO);

        public bool InsertMeasurement(string plotCode, string species, string coverage, int inventoryId);
        public List<Measurement> GetMeasurementsForPlot(int inventoryID, string code);
        public void InsertSpecies(Species species);
        public bool DeleteInventory(int inventoryId);

        public CampusDTO GetCampusDTO(int inventoryID,string campus);
    }
}
