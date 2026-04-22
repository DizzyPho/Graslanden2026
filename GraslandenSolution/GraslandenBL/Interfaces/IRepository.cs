using GraslandenBL.Domain;
using GraslandenBL.DTOs;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public void ImportInventory(Inventory inventory);

        public List<Species> GetAllSpecies();

        public Dictionary<Plot,string> GetAllGrassPlots(int inventoryID);

        public HashSet<string> GetAllCampuses();

        public List<InventoryDTO> GetInventoryDTOs();
        public int ImportEmptyInventory(InventoryDTO inventoryDTO);
        public List<Measurement> GetMeasurementsForPlot(int inventoryID, string code);
    }
}
