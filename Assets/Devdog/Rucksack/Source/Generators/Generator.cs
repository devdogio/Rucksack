using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Generators
{
    public class Generator<T> : IGenerator<T>
        where T : IEquatable<T>
    {
        private IEnumerable<T> _source;

        public Generator()
        {
            SetSource(new T[0]);
        }
        
        public void SetSource(IEnumerable<T> source)
        {
            _source = source ?? new T[0];
        }

        public IEnumerable<T> Generate()
        {
            foreach (var item in _source)
            {
                // TODO: Implement selectors
            }
            
            throw new NotImplementedException();
        }
    }
}