using GraslandenBL.Interfaces;
using GraslandenDL.FileReaders;

namespace GraslandenUtil.Factories
{
    public static class FileReaderFactory
    {
        public static IFileReader CreateFileReader(string indicatorValuesPath, string fileType)
        {
            switch (fileType.Trim().ToUpper())
            {
                case "TXT":
                    {
                        return new FileReaderTXT(indicatorValuesPath);
                    }
                default:
                    {
                        throw new NotImplementedException("Bestandstype niet ondersteund.");
                    }
            }
        }
    }
}
