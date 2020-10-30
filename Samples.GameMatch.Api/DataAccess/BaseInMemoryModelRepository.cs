using System;

namespace Samples.GameMatch.Api
{
    public abstract class BaseInMemoryModelRepository<T> : BaseInMemoryRepository<T, Guid>, IBaseModelRepository<T>
        where T : BaseModel
    {
        public override Guid Add(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id != default)
            {
                throw new ArgumentOutOfRangeException(nameof(model));
            }

            model.Id = Guid.NewGuid();

            OnAddOrUpdate(model);

            return base.Add(model);
        }

        public override T Update(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id == default)
            {
                throw new ArgumentOutOfRangeException(nameof(model));
            }

            if (!_dbModels.TryGetValue(model.Id, out var existing))
            {
                throw new ApplicationException($"Record does not exist [{typeof(T).Name}].[{model.Id}]");
            }

            model.CreatedOn = existing.CreatedOn;

            OnAddOrUpdate(model);

            return base.Update(model);
        }

        private void OnAddOrUpdate(T model)
        {
            model.ModifiedOn = DateTime.UtcNow;

            if (model.CreatedOn == default)
            {
                model.CreatedOn = model.ModifiedOn;
            }
        }
    }
}
