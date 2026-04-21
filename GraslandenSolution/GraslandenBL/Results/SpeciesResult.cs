using GraslandenBL.Domain;

namespace GraslandenBL.Results
{
    public class SpeciesResult
    {
        public Species? Object { get; private init; }

        public List<string> Errors { get; private init; }

        public SpeciesResult(Species? result, List<string> errors)
        {
            Object = result;
            Errors = errors;
        }

        //public SpeciesResult(List<string> errors)
        //{
        //    Errors = errors;
        //}
    }
}