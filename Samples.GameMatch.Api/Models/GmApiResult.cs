using System.Collections.Generic;

namespace Samples.GameMatch.Api
{
    public class GmApiResult<T> : IHasResult<T>
    {
        public T Result { get; set; }
    }

    public class GmApiResults<T> : IHasResults<T>
        where T : class
    {
        public IReadOnlyList<T> Results { get; set; }
        public int ResultCount => Results?.Count ?? 0;
    }
}
