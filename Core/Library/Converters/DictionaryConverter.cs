using System.Collections.Generic;

namespace Dose.Core.Library.Converters
{
    public abstract class DictionaryConverter<T>
    {
        public abstract T FromDict(Dictionary<string, object?> dict);
        public abstract Dictionary<string, object?> ToDict(T obj);
    }
}