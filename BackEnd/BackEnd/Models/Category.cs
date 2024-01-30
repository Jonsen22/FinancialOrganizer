using Organizer.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organizer.Models
{
    public class Category
    {
        public required int CategoryId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? UserId { get; set; }
        public required string? Name { get; set; }
        public string? Description { get; set; }
        

    }
}
