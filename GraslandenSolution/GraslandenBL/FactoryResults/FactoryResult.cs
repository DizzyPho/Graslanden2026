using System;
using System.Collections.Generic;
using System.Text;

namespace GraslandenBL.FactoryResults
{
    public class FactoryResult<T>
    {
        public T Result { get; private set; }
        public List<String> Errors { get; init; }
        public bool IsSuccess { get => Errors.Count == 0; }

        public FactoryResult(T result)
        {
            Result = result;
        }

        public FactoryResult(List<String> errors)
        {
            Errors = errors;
        }
    }
}
