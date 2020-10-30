using System;

namespace Samples.GameMatch.Api
{
    public interface IHasId<T>
    {
        public T Id { get; set; }
    }

    public interface IHasGuidId : IHasId<Guid> { }
}
