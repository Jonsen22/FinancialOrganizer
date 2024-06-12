
using System.ComponentModel.DataAnnotations.Schema;

namespace Organizer.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Colorhex { get; set; }
        

    }
}
