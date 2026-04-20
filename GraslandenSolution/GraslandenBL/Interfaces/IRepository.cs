using GraslandenBL.Domain;
using GraslandenBL.DTOs;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public void ImportInventory(List<Inventory> data);

        public List<Species> GetAllSpecies();

        public List<Inventory> GetAllInventory();

        public Dictionary<Plot,string> GetAllGrassPlots();

        public List<string> GeAllCampuses();

        public List<InventoryDTO> GetInventoriesDTOs();
    }
}
