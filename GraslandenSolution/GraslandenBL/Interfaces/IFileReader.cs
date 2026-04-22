using GraslandenBL.Domain;
using GraslandenBL.Enums;

namespace GraslandenBL.Interfaces
{
    public interface IFileReader
    {
        public List<Measurement> ReadFile(string inventoryPath, out Dictionary<string, MessageType> messages);
    }
}
