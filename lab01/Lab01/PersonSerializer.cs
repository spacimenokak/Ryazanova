using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab01
{
    public class PersonSerializer
    {
        private readonly JsonSerializerOptions _options;
        private readonly string _logFilePath;
        private readonly object _fileLock = new object();

        public PersonSerializer(string logFilePath = "log.txt")
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _logFilePath = logFilePath;
        }

        // 1. Сериализация в строку
        public string SerializeToJson(Person person)
        {
            try
            {
                return JsonSerializer.Serialize(person, _options);
            }
            catch (Exception ex)
            {
                LogError("SerializeToJson", ex);
                throw;
            }
        }

        // 2. Десериализация из строки
        public Person DeserializeFromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Person>(json, _options);
            }
            catch (Exception ex)
            {
                LogError("DeserializeFromJson", ex);
                throw;
            }
        }

        // 3. Сохранение в файл (синхронно)
        public void SaveToFile(Person person, string filePath)
        {
            try
            {
                var json = SerializeToJson(person);
                lock (_fileLock)
                {
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                LogError("SaveToFile", ex);
                throw;
            }
        }

        // 4. Загрузĸа из файла (синхронно)
        public Person LoadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found", filePath);
                }

                string json;
                lock (_fileLock)
                {
                    json = File.ReadAllText(filePath, Encoding.UTF8);
                }

                return DeserializeFromJson(json);
            }
            catch (Exception ex)
            {
                LogError("LoadFromFile", ex);
                throw;
            }
        }

        // 5. Сохранение в файл (асинхронно)
        public async Task SaveToFileAsync(Person person, string filePath)
        {
            try
            {
                var json = SerializeToJson(person);

                // атомарная запись через временный файл
                var tempFile = filePath + ".tmp";
                await File.WriteAllTextAsync(tempFile, json, Encoding.UTF8).ConfigureAwait(false);

                lock (_fileLock)
                {
                    if (File.Exists(filePath))
                    {
                        File.Replace(tempFile, filePath, null);
                    }
                    else
                    {
                        File.Move(tempFile, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("SaveToFileAsync", ex);
                throw;
            }
        }

        // 6. Загрузĸа из файла (асинхронно)
        public async Task<Person> LoadFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found", filePath);
                }

                string json;
                // чтение можно без lock, если только читаем
                json = await File.ReadAllTextAsync(filePath, Encoding.UTF8).ConfigureAwait(false);

                return DeserializeFromJson(json);
            }
            catch (Exception ex)
            {
                LogError("LoadFromFileAsync", ex);
                throw;
            }
        }

        // 7. Эĸспорт несĸольĸих объеĸтов в файл
        public void SaveListToFile(List<Person> people, string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(people, _options);
                lock (_fileLock)
                {
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                LogError("SaveListToFile", ex);
                throw;
            }
        }

        // 8. Импорт из файла
        public List<Person> LoadListFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found", filePath);
                }

                string json;
                lock (_fileLock)
                {
                    json = File.ReadAllText(filePath, Encoding.UTF8);
                }

                return JsonSerializer.Deserialize<List<Person>>(json, _options) ?? new List<Person>();
            }
            catch (Exception ex)
            {
                LogError("LoadListFromFile", ex);
                throw;
            }
        }

        private void LogError(string method, Exception ex)
        {
            try
            {
                var message = $"{DateTime.Now:O} [{method}] {ex}\n";
                lock (_fileLock)
                {
                    File.AppendAllText(_logFilePath, message, Encoding.UTF8);
                }
            }
            catch
            {
                // логгер не должен падать дальше
            }
        }
    }
}
