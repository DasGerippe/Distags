using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Distags.Core.Management;

public abstract class SettingManager<TValue>
{
    private readonly DistagsDbContext _DbContext;

    protected abstract string SettingKey { get; }

    protected abstract TValue DefaultValue { get; }

    protected SettingManager(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public virtual async Task SetSettingValueAsync(TValue value)
    {
        Setting? setting = await LoadSetting().ConfigureAwait(false);
        if (setting == null)
        {
            setting = new Setting { Key = SettingKey };
            _DbContext.Add(setting);
        }

        setting.Value = JsonSerializer.Serialize(value);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task<TValue?> GetSettingValueAsync()
    {
        Setting? setting = await LoadSetting().ConfigureAwait(false);
        if (setting == null)
            return DefaultValue;

        TValue? value = setting.Value == null ? default : JsonSerializer.Deserialize<TValue>(setting.Value);
        return value;
    }

    private Task<Setting?> LoadSetting()
    {
        return _DbContext.Set<Setting>().FirstOrDefaultAsync(setting => setting.Key == SettingKey);
    }
}
