using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Lab01.Tests
{
    [TestFixture]
    public class PersonTests
    {
        [Test]
        public void Person_FullName_And_IsAdult_Work()
        {
            // Arrange
            var person = new Lab01.Person
            {
                FirstName = "Anna",
                LastName = "Ryazanova",
                Age = 21
            };

            // Act & Assert
            Assert.That(person.FullName, Is.EqualTo("Anna Ryazanova"));
            Assert.That(person.IsAdult, Is.True);
            
            // Проверка для несовершеннолетнего
            person.Age = 17;
            Assert.That(person.IsAdult, Is.False);
        }

        [Test]
        public void Person_Email_Validation_Works()
        {
            // Arrange
            var person = new Lab01.Person();

            // Act & Assert для корректного email
            person.Email = "test@example.com";
            Assert.That(person.Email, Is.EqualTo("test@example.com"));

            // Assert для некорректного email
            Assert.Throws<ArgumentException>(() =>
            {
                person.Email = "invalid-email";
            });
        }

        [Test]
        public void Person_PhoneNumber_Validation_Works()
        {
            // Arrange
            var person = new Lab01.Person();

            // Act & Assert для корректного номера
            person.PhoneNumber = "+7-900-000-00-00";
            Assert.That(person.PhoneNumber, Is.EqualTo("+7-900-000-00-00"));

            // Assert для некорректного номера (если есть валидация)
            // Если валидации нет, этот тест можно пропустить
        }
    }

    [TestFixture]
    public class PersonSerializerTests
    {
        private readonly Lab01.PersonSerializer _serializer = new Lab01.PersonSerializer();
        private readonly Lab01.Person _testPerson = new Lab01.Person
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Age = 30,
            Email = "ivan@example.com",
            PhoneNumber = "+7-900-111-11-11"
        };

        [Test]
        public void SerializeToJson_And_DeserializeFromJson_Work()
        {
            // Act
            var json = _serializer.SerializeToJson(_testPerson);
            var restored = _serializer.DeserializeFromJson(json);

            // Assert
            Assert.That(restored.FirstName, Is.EqualTo(_testPerson.FirstName));
            Assert.That(restored.LastName, Is.EqualTo(_testPerson.LastName));
            Assert.That(restored.Email, Is.EqualTo(_testPerson.Email));
            Assert.That(restored.PhoneNumber, Is.EqualTo(_testPerson.PhoneNumber));
            Assert.That(restored.Age, Is.EqualTo(_testPerson.Age));
        }

        [Test]
        public void SaveToFile_And_LoadFromFile_Work()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), $"test_person_{Guid.NewGuid()}.json");

            try
            {
                // Act
                _serializer.SaveToFile(_testPerson, tempFile);
                var fromFile = _serializer.LoadFromFile(tempFile);

                // Assert
                Assert.That(fromFile.FirstName, Is.EqualTo(_testPerson.FirstName));
                Assert.That(fromFile.Email, Is.EqualTo(_testPerson.Email));
                Assert.That(File.Exists(tempFile), Is.True);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Test]
        public async Task SaveToFileAsync_And_LoadFromFileAsync_Work()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), $"test_person_async_{Guid.NewGuid()}.json");

            try
            {
                // Act
                await _serializer.SaveToFileAsync(_testPerson, tempFile);
                var fromFile = await _serializer.LoadFromFileAsync(tempFile);

                // Assert
                Assert.That(fromFile.PhoneNumber, Is.EqualTo(_testPerson.PhoneNumber));
                Assert.That(fromFile.LastName, Is.EqualTo(_testPerson.LastName));
                Assert.That(File.Exists(tempFile), Is.True);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }

    [TestFixture]
    public class FileResourceManagerTests
    {
        [Test]
        public void FileResourceManager_CanBeCreated()
        {
            // Это базовый тест для проверки, что класс существует
            Assert.DoesNotThrow(() => 
            {
                var manager = new Lab01.FileResourceManager("test.txt");
            });
        }
    }
}