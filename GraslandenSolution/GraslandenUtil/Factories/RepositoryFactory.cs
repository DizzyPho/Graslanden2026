using GraslandenBL.Interfaces;
using GraslandenDL.Repositories;

namespace GraslandenUtil.Factories
{
    public static class RepositoryFactory
    {
        public static IRepository CreateRepository(string connectionString, string databaseType)
        {
            switch(databaseType.Trim().ToUpper())
            {
                case "SQL":
                    {
                        return new RepositorySQL(connectionString);
                    }
                default:
                    {
                        throw new NotImplementedException("Databanktype niet ondersteund.");
                    }
            }
        }
    }
}
