# Генератор репозиториев для IUnitOfWorkProvider

Позволяет сгенерировать репозиторий данных с использованием `IUnitOfWorkProvider`, методы которого вызывают функции или процедуры базы данных.

Репозиторий описывается через публичный интерфейс:

```csharp
[UnitOfWork("MyUnitOfWork", MethodPrefix="fn_", ParameterPrefix="p_")]
public interface ISettingsRepository
{
    Task<int> GetSettingsCountAsync();
    Task<List<SettingsEntity>> GetSettingsByFilterAsync(string filter);
}
```

Чтобы создать репозиторий используем метод `IUnitOfWorkProvider.Repository<>(string, CancellationToken)`:

```csharp
var settings = await unitOfWorkProvider
    .Repository<ISettingsRepository>(cancellationToken)
    .GetSettingsAssync(filter);

```

Код созданного репозитория будет аналогичен следующему:

```csharp
public class SettingsRepository : UnitOfWorkTarget, ISettingsRepository
{
    public Task<int> GetSettingsCountAsync()
    {
        return UnitOfWorkProvider
            .GetUnitOfWork(Name, СancellationToken)
            .CallScalarFunctionAsync(
                "fn_get_settings_count",
                new Dictionary<string, object>());
    }

    public Task<List<SettingsEntity>> GetSettingsByFilterAsync(string settingsFilter)
    {
        return UnitOfWorkProvider
            .GetUnitOfWork(Name, СancellationToken)
            .CallTableFunctionAsync(
                "fn_get_settings_by_filter",
                new Dictionary<string, object>()
                {
                    ["p_settings_filter"] = settingsFilter,
                });
    }
}
```