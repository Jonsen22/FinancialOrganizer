using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Organizer.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public float? Balance { get; set; }
    }
}
