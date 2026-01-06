# Лабораторная работа 2

Тема: работа с коллекциями (List, LinkedList, Queue, Stack, ImmutableList).

## Цель

Проанализировать и сравнить производительность основных операций над коллекциями, а также проверить корректность их работы с помощью модульных тестов.

## Реализация

- Основной проект `Lab02`:
  - Класс `CollectionBenchmark` выполняет операции добавления, удаления, поиска и доступа по индексу (где применимо) для `List<int>`, `LinkedList<int>`, `Queue<int>`, `Stack<int>`, `ImmutableList<int>`.
  - Используется `Stopwatch` для замера времени, каждая операция выполняется не менее 5 раз, результаты усредняются.
  - Результаты выводятся в табличной форме в консоль.

- Тестовый проект `Lab02.Tests` (NUnit):
  - Тесты проверяют корректность добавления, удаления и поиска элементов для каждой коллекции.
  - Тесты детерминированы: фиксированное количество элементов и ожидаемые результаты.

## Используемые технологии

- .NET SDK, C#.
- Коллекции `System.Collections.Generic` и `System.Collections.Immutable`.
- Фреймворк тестирования NUnit.

## Инструкция по запуску

```bash
cd lab02

# запуск тестов
dotnet test

# запуск замеров производительности
dotnet run --project Lab02/Lab02.csproj

## Результаты измерений
Collection      | Operation            |     Avg ms
-------------------------------------------------------
ImmutableList   | Add(end)             |      0,046
ImmutableList   | Index access         |     19,130
ImmutableList   | Insert(begin)        |      0,109
ImmutableList   | Insert(middle)       |      0,024
ImmutableList   | Remove(begin)        |      0,121
ImmutableList   | Remove(end)          |      0,022
ImmutableList   | Remove(middle)       |      0,020
ImmutableList   | Search(value)        |      3,279
LinkedList      | Add(middle)          |      6,818
LinkedList      | AddFirst             |      8,692
LinkedList      | AddLast              |     11,368
LinkedList      | Remove(middle)       |      5,954
LinkedList      | RemoveFirst          |      6,602
LinkedList      | RemoveLast           |      8,241
LinkedList      | Search(value)        |      0,898
List            | Add(end)             |      0,402
List            | Index access         |      0,945
List            | Insert(begin)        |      0,510
List            | Insert(middle)       |      0,537
List            | RemoveAt(begin)      |      0,189
List            | RemoveAt(end)        |      0,273
List            | RemoveAt(middle)     |      0,297
List            | Search(value)        |      0,367
ImmutableList   | Remove(middle)       |      0,020
ImmutableList   | Search(value)        |      3,279
LinkedList      | Add(middle)          |      6,818
LinkedList      | AddFirst             |      8,692
LinkedList      | AddLast              |     11,368
LinkedList      | Remove(middle)       |      5,954
LinkedList      | RemoveFirst          |      6,602
LinkedList      | RemoveLast           |      8,241
LinkedList      | Search(value)        |      0,898
List            | Add(end)             |      0,402
List            | Index access         |      0,945
List            | Insert(begin)        |      0,510
List            | Insert(middle)       |      0,537
List            | RemoveAt(begin)      |      0,189
List            | RemoveAt(end)        |      0,273
List            | RemoveAt(middle)     |      0,297
List            | Search(value)        |      0,367
LinkedList      | AddLast              |     11,368
LinkedList      | Remove(middle)       |      5,954
LinkedList      | RemoveFirst          |      6,602
LinkedList      | RemoveLast           |      8,241
LinkedList      | Search(value)        |      0,898
List            | Add(end)             |      0,402
List            | Index access         |      0,945
List            | Insert(begin)        |      0,510
List            | Insert(middle)       |      0,537
List            | RemoveAt(begin)      |      0,189
List            | RemoveAt(end)        |      0,273
List            | RemoveAt(middle)     |      0,297
List            | Search(value)        |      0,367
List            | Add(end)             |      0,402
List            | Index access         |      0,945
List            | Insert(begin)        |      0,510
List            | Insert(middle)       |      0,537
List            | RemoveAt(begin)      |      0,189
List            | RemoveAt(end)        |      0,273
List            | RemoveAt(middle)     |      0,297
List            | Search(value)        |      0,367
List            | Insert(middle)       |      0,537
List            | RemoveAt(begin)      |      0,189
List            | RemoveAt(end)        |      0,273
List            | RemoveAt(middle)     |      0,297
List            | Search(value)        |      0,367
List            | RemoveAt(middle)     |      0,297
List            | Search(value)        |      0,367
List            | Search(value)        |      0,367
Queue           | Dequeue              |      1,164
Queue           | Enqueue              |      2,151
Queue           | Search(value)        |      0,116
Stack           | Pop                  |      0,940
Stack           | Push                 |      1,680
Stack           | Search(value)        |      0,241

## Выводы

1. **List<T>** показывает наилучшую сбалансированную производительность для большинства операций.
2. **LinkedList<T>** эффективен только для операций в начале/конце списка.
3. **ImmutableList<T>** имеет высокую стоимость операций изменения из-за создания новых версий.
4. **Queue<T> и Stack<T>** специализированы для своих сценариев (FIFO/LIFO).