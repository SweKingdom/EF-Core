using ConsoleApp2;
using ConsoleApp2.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));
// Säkerhetställ DB + migration + Seed
using (var db = new ShopContext())
{
    // Migrate Aync: Skapar databasen om den inte finns
    // Kör bara om det inte finns några kategorier sen innan
    await db.Database.MigrateAsync();

    // Enkel seeding för Databasen
    // Kör bara om det inte finns några kategorier sen innan
    if (!await db.Categories.AnyAsync())
    {
        db.Categories.AddRange(
            new Category {CategoryName = "Books", CategoryDescription = "All books"},
            new Category {CategoryName = "Movies", CategoryDescription = "All movies"}
            );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
}

// CLI för CRUD; CREATE, READ, UPDATE DELETE
while (true)
{
    Console.WriteLine("\nCommands: List | add | delete <id> | edit <id> | exit");
    Console.Write("> ");
    var line = Console.ReadLine()?.Trim() ?? string.Empty;
    // Hoppa över tomma rader
    if (string.IsNullOrEmpty(line))
    {
        continue;
    }

    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break; // Avsluta programmet, hoppa ur loopen
    }
    
    // Delar upp raden på mellanslag: tex "edit 2" --> ["edit", "2"]
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var cmd = parts[0].ToLowerInvariant();
    
    // Enkel switch för kommandotolkning
    switch (cmd)
    {
        case "list":
            // Lista våra categories
            await ListAsync();
            break;
        case "add":
            // Lägg till en category
            await AddAsync();
            break;
        case "edit":
            // Redigera en category
            break;
        case "delete":
            // Radera en category
            break;
        default:
            Console.WriteLine($"Unknown command.");
            break;
            
    }
}
// READ: Lista alla kategorier
static async Task ListAsync()
{
    var db = new ShopContext();
    
    // AsNoTracking = snabare för read-only scenarion. ( Ingen change tracking)
    var rows = await db.Categories.AsNoTracking().OrderBy(category => category.CategoryName).ToListAsync();
    Console.WriteLine("Id | Name | Description ");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.CategoryId} | {row.CategoryName} | {row.CategoryDescription} ");
    }
}

//CREATE: Lägg till en ny kategori
static async Task AddAsync()
{
    Console.WriteLine("Name: ");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;
    
    // Enkel validering
    if (string.IsNullOrEmpty(name) || name.Length > 100)
    {
        Console.WriteLine("Name is required (max 100).");
        return;
    }
    Console.WriteLine("Description (optional): ");
    var desc = Console.ReadLine()?.Trim() ?? string.Empty;
    
    using var db = new ShopContext();
    await db.Categories.AddAsync(new Category { CategoryName = name, CategoryDescription = desc });
    try
    {
        // Spara våra ändringar; Trigga en INSERT + all validering/constraints i databasen
        await db.SaveChangesAsync();
        Console.WriteLine("Category added");
    }
    catch (DbUpdateException exception)
    {
        // Hit kommer vi tex om UNIQUE - Indexet op CategoryName bryts
        Console.WriteLine("Db Error (Maby duplicate?)! "+ exception.GetBaseException().Message);
    }
}