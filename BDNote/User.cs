namespace BDNote;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string Email { get; set; }
    public List<Note> Notes { get; set; } = [];
}