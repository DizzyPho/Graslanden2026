using GraslandenBL.Domain;
using GraslandenBL.Interfaces;

namespace GraslandenBL.Managers
{
    public class ImportManager
    {
        private IRepository _repository;
        private IFileReader _fileReader;

        public ImportManager(IRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
        }

        public List<Measurement> ReadFile()
        {
            return _fileReader.ReadFile();
        }
    }
}
