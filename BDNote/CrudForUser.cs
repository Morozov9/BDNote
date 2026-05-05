namespace BDNote;

using Microsoft.EntityFrameworkCore;

public class CrudForUser
{
    /// <summary>
    /// Создаёт нового пользователя и сохраняет его в БД.
    /// </summary>
    /// <returns>
    /// Сущность нового пользователя. После сохранения в БД его свойство <see cref="User.Id"/>
    /// будет содержать реальный ID из СУБД (не 0).
    /// </returns>
    public static async Task<User> Create(string username, string email, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var user = new User
        {
            Email = email,
            Username =  username,
        };
        
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    /// <summary>
    /// Получает список пользователей с поиском по частичному совпадению имени.
    /// </summary>
    public static async Task<List<User>> Read(string searchText, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Users
            .Where(x => EF.Functions.Like(x.Username, $"%{searchText}%"))
            .AsNoTracking() 
            .ToListAsync(ct);
    }
    
    /// <summary>
    /// Ищет конкретного пользоваетеля по его ID.
    /// </summary>
    /// <returns>
    /// Сущность найденного пользователя или <see langword="null"/> если такого нет.
    /// </returns>
    public static async Task<User?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    
    /// <summary>
    /// Получает пользователя вместе со всеми его заметками.
    /// </summary>
    public static async Task<User?> ReadUserWithNotes(int userId, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Users
            .Include(u => u.Notes)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }
    
    /// <summary>
    /// Обновляет сущность пользователя в БД. Меняет данные в той же сущности, не создаёт новый инстанс.
    /// </summary>
    public static async Task Update(User user, string username, string email, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        user.Username = username;
        user.Email = email;
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Удаляет сущность пользователя из БД.
    /// </summary>
    public static async Task Delete(User user, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}