namespace BDNote;

public class Note
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}