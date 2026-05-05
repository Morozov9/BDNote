namespace BDNote;

using Microsoft.EntityFrameworkCore;

public class Crud
{
    /// <summary>
    /// Создаёт новую заметку и сохраняет его в БД.
    /// </summary>
    /// <returns>
    /// Сущность новой заметки. После сохранения в БД её свойство <see cref="Student.Id"/>
    /// будет содержать реальный ID из СУБД (не 0).
    /// </returns>
    public static async Task<Note> Create(int userid, string text, DateTimeOffset createdAt, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var note = new Note
        {
            Text = text,
            CreatedAt = createdAt,
            UserId = userid,
        };
        
        db.Notes.Add(note);
        await db.SaveChangesAsync(ct);
        return note;
    }

    /// <summary>
    /// Получает список заметок с поиском по частичному совпадению текста.
    /// </summary>
    public static async Task<List<Note>> Read(string searchText, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Notes
            .Where(x => EF.Functions.Like(x.Text, $"%{searchText}%"))
            .AsNoTracking() 
            .ToListAsync(ct);
    }

    /// <summary>
    /// Ищет конкретную заметку по его ID.
    /// </summary>
    /// <returns>
    /// Сущность найденной заметки или <see langword="null"/> если такого нет.
    /// </returns>
    public static async Task<Note?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Notes.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    /// <summary>
    /// Обновляет сущность заметки в БД. Меняет данные в той же сущности, не создаёт новый инстанс.
    /// </summary>
    public static async Task Update(Note note, string text, DateTimeOffset createdAt, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        note.Text = text;
        note.CreatedAt = createdAt;
        db.Notes.Update(note);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Удаляет сущность заметки из БД.
    /// </summary>
    public static async Task Delete(Note note, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Notes.Remove(note);
        await db.SaveChangesAsync(ct);
    }
}