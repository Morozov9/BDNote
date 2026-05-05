namespace BDNote.Tests;
[Collection("Sequential")]
public class CrudTests:IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await using var db = new DataContext();
        // Полная очистка перед каждым тестом
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateNote_WhenUserExists_ShouldLinkCorrectlty()
    {
        // --- Arrange ---
        var username = "CsharpDeveloper";
        var email = "dotnet@example.com";
        var noteText = "Изучить паттерн AAA";
        var timestamp = DateTimeOffset.UtcNow;

        // --- Act ---
        var user = await CrudForUser.Create(username, email);
        var note = await Crud.Create(user.Id, noteText, timestamp);
        var result = await CrudForUser.ReadUserWithNotes(user.Id);

        // --- Assert ---
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.Single(result.Notes);
        Assert.Equal(noteText, result.Notes[0].Text);
        Assert.Equal(user.Id, result.Notes[0].UserId);
    }

    [Fact]
    public async Task DeleteUser_ShouldAutomaticallyDeleteHisNotes()
    {
        // --- Arrange ---
        var user = await CrudForUser.Create("TemporaryUser", "temp@test.com");
        var note = await Crud.Create(user.Id, "Эта заметка скоро исчезнет", DateTimeOffset.UtcNow);

        // --- Act ---
        await CrudForUser.Delete(user);

        // --- Assert ---
        var noteInDb = await Crud.Read(note.Id);
        Assert.Null(noteInDb);
    }

    [Fact]
    public async Task UpdateUser_ShouldPersistChangesInDatabase()
    {
        // --- Arrange ---
        var user = await CrudForUser.Create("OldName", "old@mail.com");
        var newName = "NewModernName";
        var newEmail = "new@mail.com";

        // --- Act ---
        await CrudForUser.Update(user, newName, newEmail);

        // --- Assert ---
        var updatedUser = await CrudForUser.Read(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Username);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task ReadUsers_BySearchText_ShouldReturnMatchingUsers()
    {
        // --- Arrange ---
        await CrudForUser.Create("Alexander", "alex@test.com");
        await CrudForUser.Create("Alexey", "alexey@test.com");
        await CrudForUser.Create("Dmitry", "dima@test.com");
        var searchPart = "Alex";

        // --- Act ---
        var results = await CrudForUser.Read(searchPart);

        // --- Assert ---
        Assert.Equal(2, results.Count);
        Assert.All(results, u => Assert.Contains(searchPart, u.Username));
        Assert.DoesNotContain(results, u => u.Username == "Dmitry");
    }
}