using GraslandenBL.Domain;
using GraslandenBL.FactoryResults;
using GraslandenBL.Interfaces;

namespace GraslandenBL.Managers
{
    internal class ImportManager
    {
        private IRepository _repository;
        private IFileReader _fileReader;

        public ImportManager(IRepository repository, IFileReader fileReader)
        {
            _repository = repository;
            _fileReader = fileReader;
        }

        public List<FactoryResult<Measurement>> ReadFile()
        {
            return _fileReader.ReadFile();
        }
    }
}
