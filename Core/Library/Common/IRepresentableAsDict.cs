using System.Collections.Generic;

namespace Dose.Core.Library.Common
{
    public interface IRepresentableAsDict<TKey, TValue> where TKey : notnull
    {
        Dictionary<TKey, TValue> AsDict();
    }
}