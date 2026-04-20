using GraslandenBL.Interfaces;
using GraslandenDL.FileReaders;

namespace GraslandenUtil.Factories
{
    public static class FileReaderFactory
    {
        public static IFileReader CreateFileReader(string inventoryFilePath, string indicatorValuesPath, string fileType)
        {
            switch (fileType.Trim().ToUpper())
            {
                case "CSV":
                    {
                        return new FileReaderCSV(inventoryFilePath, indicatorValuesPath);
                    }
                default:
                    {
                        throw new NotImplementedException("Bestandstype niet ondersteund.");
                    }
            }
        }
    }
}
