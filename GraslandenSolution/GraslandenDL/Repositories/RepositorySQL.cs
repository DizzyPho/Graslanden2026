using GraslandenBL.Domain;
using GraslandenBL.Interfaces;

namespace GraslandenDL.Repositories
{
    public class RepositorySQL : IRepository
    {
        private string _connectionString;

        public RepositorySQL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Species> GetAllSpecies()
        {
            throw new NotImplementedException();
        }

        public void ImportInventory()
        {
            throw new NotImplementedException();
        }
    }
}
