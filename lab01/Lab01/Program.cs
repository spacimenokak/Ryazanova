using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lab01
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var serializer = new PersonSerializer("log.txt");

            var person = new Person
            {
                FirstName = "Anna",
                LastName = "Ryazanova",
                Age = 21,
                Password = "secret",
                Id = Guid.NewGuid().ToString(),
                BirthDate = new DateTime(2004, 5, 1),
                Email = "anna@example.com",
                PhoneNumber = "+7-900-000-00-00"
            };

            var filePath = "person.json";
            var listFilePath = "people.json";

            try
            {
                // 1–2. сериализация / десериализация
                var json = serializer.SerializeToJson(person);
                Console.WriteLine("Serialized person:");
                Console.WriteLine(json);

                var restoredFromJson = serializer.DeserializeFromJson(json);
                Console.WriteLine($"Restored from string: {restoredFromJson.FullName}, adult: {restoredFromJson.IsAdult}");

                // 3–4. синхронная работа с файлом
                serializer.SaveToFile(person, filePath);
                var fromFile = serializer.LoadFromFile(filePath);
                Console.WriteLine($"Loaded from file: {fromFile.FullName}, email: {fromFile.Email}");

                // 5–6. асинхронная работа с файлом
                await serializer.SaveToFileAsync(person, "person_async.json");
                var fromFileAsync = await serializer.LoadFromFileAsync("person_async.json");
                Console.WriteLine($"Loaded async: {fromFileAsync.FullName}, phone: {fromFileAsync.PhoneNumber}");

                // 7–8. список объектов
                var people = new List<Person>
                {
                    person,
                    new Person
                    {
                        FirstName = "Ivan",
                        LastName = "Petrov",
                        Age = 17,
                        Password = "pwd",
                        Id = Guid.NewGuid().ToString(),
                        BirthDate = new DateTime(2007, 1, 10),
                        Email = "ivan@example.com",
                        PhoneNumber = "+7-900-111-11-11"
                    }
                };

                serializer.SaveListToFile(people, listFilePath);
                var loadedList = serializer.LoadListFromFile(listFilePath);
                Console.WriteLine($"Loaded list count: {loadedList.Count}");

                // Тестирование FileResourceManager
                var textFilePath = "test.txt";

                using (var manager = new FileResourceManager(textFilePath))
                {
                    manager.OpenForWriting();
                    manager.WriteLine("Первая строка");
                    manager.WriteLine("Вторая строка");
                }

                using (var manager = new FileResourceManager(textFilePath))
                {
                    manager.AppendText("\nТекст, добавленный через AppendText");
                    var text = manager.ReadAllText();
                    Console.WriteLine("File content:");
                    Console.WriteLine(text);

                    var info = manager.GetFileInfo();
                    Console.WriteLine($"File size: {info.Length} bytes, created: {info.CreationTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Done.");
        }
    }
}
