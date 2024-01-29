using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Organizer.Models
{
    public class BankAccount
    {
        public required int BankAccountId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public required string? UserId { get; set; }
        public required string? Name { get; set; }
        public float? Balance { get; set; }
    }
}
