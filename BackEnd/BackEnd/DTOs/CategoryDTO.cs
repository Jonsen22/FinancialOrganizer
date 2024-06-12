namespace BackEndTest.DTOs
{
    public class CategoryDTO
    {
        public required int CategoryId { get; set; }
        public required string? Name { get; set; }
        public string? Description { get; set; }
        public string? Colorhex { get; set; }
    }
}
