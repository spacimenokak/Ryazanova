# Лабораторная работа 1

Тема: сериализация JSON, управление ресурсами и работа с файлами в C#.

## Цели работы

- Освоить использование атрибутов JSON при сериализации и десериализации объектов.
- Научиться работать с файловой системой и управлять ресурсами через IDisposable.
- Реализовать синхронные и асинхронные операции чтения/записи файлов JSON.

## Реализация

- Класс `Person`:
  - Свойства с атрибутами `[JsonIgnore]`, `[JsonPropertyName]`, `[JsonInclude]`.
  - Валидация `Email` в сеттере.
  - Только для чтения `FullName` и `IsAdult`.

- Класс `PersonSerializer`:
  - Сериализация/десериализация объектов `Person`.
  - Сохранение и загрузка объектов в/из файлов (синхронно и асинхронно).
  - Работа со списком объектов.
  - Логирование ошибок в файл с помощью `JsonSerializerOptions` с `WriteIndented = true`.

- Класс `FileResourceManager`:
  - Управление `FileStream`, `StreamWriter`, `StreamReader`.
  - Методы `OpenForWriting`, `OpenForReading`, `WriteLine`, `ReadAllText`, `AppendText`, `GetFileInfo`.
  - Корректная реализация паттерна `IDisposable`, финализатор, защита от повторного использования после `Dispose`.

## Используемые технологии

- .NET SDK (консольное приложение).
- Пространства имён:
  - `System.Text.Json`, `System.Text.Json.Serialization`.
  - `System.IO`.
  - `System.Threading.Tasks`.

## Инструкция по запуску

```bash
cd lab01/Lab01
dotnet build
dotnet run
