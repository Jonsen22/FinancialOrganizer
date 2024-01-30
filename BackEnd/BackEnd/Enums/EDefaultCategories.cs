using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Organizer.Enums
{

        [JsonConverter(typeof(StringEnumConverter))]
        public enum DefaultCategories
    {
        [EnumMember(Value = "Groceries")]
        Groceries = 1,

    }
    
}
