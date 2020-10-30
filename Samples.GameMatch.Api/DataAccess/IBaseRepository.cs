using System;
using System.Collections.Generic;

namespace Samples.GameMatch.Api
{
    public interface IBaseRepository<T, TIdType>
        where T : class, IHasId<TIdType>
    {
        T GetById(TIdType id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Query(Predicate<T> predicate);
        TIdType Add(T model);
        T Update(T model);
        void Delete(TIdType id);
    }

    public interface IBaseModelRepository<T> : IBaseRepository<T, Guid>
        where T : BaseModel { }
}
