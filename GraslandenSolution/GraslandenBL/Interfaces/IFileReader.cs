using GraslandenBL.Domain;

namespace GraslandenBL.Interfaces
{
    public interface IFileReader
    {
        public FactoryResult<Measurement> ReadFile();
    }
}
