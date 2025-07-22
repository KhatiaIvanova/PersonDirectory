namespace PersonDirectory.Application.DTOs
{
    public class PersonSearchRequestDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? PersonalNumber { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
