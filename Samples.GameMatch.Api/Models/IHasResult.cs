using System.Collections.Generic;

namespace Samples.GameMatch.Api
{
    public interface IHasResult<out T>
    {
        T Result { get; }
    }

    public interface IHasResults<out T>
        where T : class
    {
        IReadOnlyList<T> Results { get; }
    }
}
