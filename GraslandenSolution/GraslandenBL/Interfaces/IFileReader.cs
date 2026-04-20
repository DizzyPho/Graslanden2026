using GraslandenBL.Domain;

namespace GraslandenBL.Interfaces
{
    public interface IFileReader
    {
        public List<Measurement> ReadFile();
    }
}
