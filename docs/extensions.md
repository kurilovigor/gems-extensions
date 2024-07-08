# Общие расширения

## Метод string.Convert()
Конвертирует строку применяя к ней правило именования. Пример:

```csharp
Console.WriteLine("GetItem".Convert(NamingConvention.None));
Console.WriteLine("GetItem".Convert(NamingConvention.CamelCase));
Console.WriteLine("GetItem".Convert(NamingConvention.SnakeCase));
Console.WriteLine("getItem".Convert(NamingConvention.PascalCase));
Console.WriteLine("GetItem".Convert(NamingConvention.KebabCase));

// GetItem
// getItem
// get_item
// GetItem
// get-item

```

## Метод NamingConvention.Convert()
Конвертирует строку применяя к ней правило именования. Пример:

```csharp
Console.WriteLine(NamingConvention.None.Convert("GetItem"));
Console.WriteLine(NamingConvention.CamelCase.Convert("GetItem"));
Console.WriteLine(NamingConvention.SnakeCase.Convert("GetItem"));
Console.WriteLine(NamingConvention.PascalCase.Convert("getItem"));
Console.WriteLine(NamingConvention.KebabCase.Convert("GetItem"));

// GetItem
// getItem
// get_item
// GetItem
// get-item

```

## Метод object.ToDictionary()

Позволяет сконвертировать любой объект в `Dictionary<string, object>`.
Пример использования:

```csharp
var dict = new 
    { 
        FirstWord = "Hello",
        SecondWord = "world",
    }.ToDictionary();

// dict["FirstWord"] = "Hello"
// dict["SecondWord"] = "world"
```

Можно применить соглашение об именовании:

```csharp
var dict = new 
    { 
        FirstWord = "Hello",
        SecondWord = "world",
    }.ToDictionary(NamingConvention.SnakeCase);

// dict["first_word"] = "Hello"
// dict["second_word"] = "world"
```
