using GraslandenBL.Domain;
using GraslandenBL.DTOs;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public void ImportInventory(List<Inventory> data);

        public List<Species> GetAllSpecies();
        public List<InventoryDTO> GetInventories();
    }
}
