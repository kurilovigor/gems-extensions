# Расширения IUnitOfWork

Расширения IUnitOfWork повторяют методы IUnitOfWork, но позволяют в качестве параметров передвавать объект. Пример:

```csharp
var settings = await unitOfWork
    .CallTableFunctionAsync(
        "get_settings_by_filter",
        new 
        {
            p_settings_filter = settingsFilter,
        });

// Аналогично параметру:
// new Dictionary<string, object>()
// {
//    ["p_settings_filter"] = settingsFilter,
// }
```