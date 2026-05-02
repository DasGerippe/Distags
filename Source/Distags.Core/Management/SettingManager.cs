using DataClasses;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Distags.Core.Management;

public abstract class SettingManager<TValue>
    where TValue : struct, IParsable<TValue>
{
    private readonly DistagsDbContext _DbContext;

    protected abstract string SettingKey { get; }

    protected abstract TValue DefaultValue { get; }

    protected SettingManager(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public virtual async Task SetSettingValue(TValue value)
    {
        Setting? setting = await LoadSetting().ConfigureAwait(false);
        if (setting == null)
        {
            setting = new Setting { Key = SettingKey };
            _DbContext.Add(setting);
        }

        setting.Value = Convert.ToString(value, CultureInfo.InvariantCulture);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task<TValue> GetSettingValue()
    {
        Setting? setting = await LoadSetting().ConfigureAwait(false);
        if (setting == null || !TValue.TryParse(setting.Value, CultureInfo.InvariantCulture, out TValue value))
            return DefaultValue;

        return value;
    }

    private Task<Setting?> LoadSetting()
    {
        return _DbContext.Set<Setting>().FirstOrDefaultAsync(setting => setting.Key == SettingKey);
    }
}