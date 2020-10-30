using System.Linq;

namespace Samples.GameMatch.Api
{
    public class InMemorySettingsRepository : BaseInMemoryModelRepository<Setting>, ISettingsRepository
    {
        private Setting _defaultSetting;

        public Setting GetDefaultSetting()
            => _defaultSetting ??= GetAll().FirstOrDefault();
    }
}
