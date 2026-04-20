using GraslandenBL.Domain;

namespace GraslandenBL.Interfaces
{
    public interface IRepository
    {
        public void ImportInventory();

        public List<Species> GetAllSpecies();
    }
}
