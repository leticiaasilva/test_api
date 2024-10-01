public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }  // "verified" or "unverified"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
