namespace BDNote.Tests;

public class CrudTests
{
    [Fact]
    public async Task TestCreate()
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync(); 

        // Act
        var note = await Crud.Create("Простая заметка", DateTimeOffset.Now);

        // Assert
        Assert.True(note.Id > 0);
        Assert.Equal("Простая заметка", note.Text);
    }
    
    [Fact]
    public async Task TestReadById()
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync(); 

        // Arrange
        var note = await Crud.Create("Найти меня", DateTimeOffset.Now);

        // Act
        var found = await Crud.Read(note.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(note.Id, found.Id);
    }

    [Fact]
    public async Task TestUpdate()
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync(); 

        // Arrange
        var note = await Crud.Create("Старый текст", DateTimeOffset.Now);

        // Act
        await Crud.Update(note, "Новый текст", DateTimeOffset.Now);

        // Assert
        var updated = await Crud.Read(note.Id);
        Assert.Equal("Новый текст", updated?.Text);
    }

    [Fact]
    public async Task TestDelete()
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync(); 

        // Arrange
        var note = await Crud.Create("Удали меня", DateTimeOffset.Now);

        // Act
        await Crud.Delete(note);

        // Assert
        var deleted = await Crud.Read(note.Id);
        Assert.Null(deleted);
    }
}