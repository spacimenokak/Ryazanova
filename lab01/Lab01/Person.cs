using System;
using System.Text.Json.Serialization;

namespace Lab01
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonPropertyName("personId")]
        public string Id { get; set; }

        [JsonInclude]
        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get => _birthDate;
            set => _birthDate = value;
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                {
                    throw new ArgumentException("Invalid email");
                }

                _email = value;
            }
        }

        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public bool IsAdult => Age >= 18;
    }
}
