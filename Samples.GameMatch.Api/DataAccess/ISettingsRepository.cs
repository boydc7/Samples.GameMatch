namespace Samples.GameMatch.Api
{
    public interface ISettingsRepository : IBaseModelRepository<Setting>
    {
        Setting GetDefaultSetting();
    }
}
