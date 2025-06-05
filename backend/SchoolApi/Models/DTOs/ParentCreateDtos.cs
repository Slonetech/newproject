// DTOs/ParentCreateDto.cs
public class ParentCreateDto
{
    public string ?Name { get; set; }
    public string ?Email { get; set; }
    // Add other relevant fields (like phone, address, etc.)
}

// DTOs/ParentUpdateDto.cs
public class ParentUpdateDto
{
    public string? Name { get; set; }
    public string ?Email { get; set; }
    // Other updatable fields
}
