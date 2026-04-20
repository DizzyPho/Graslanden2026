using GraslandenBL.Domain;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public void ImportInventory(List<Inventory> data);

        public List<Species> GetAllSpecies();

    }
}
