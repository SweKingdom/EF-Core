using System.Diagnostics.Metrics;using ConsoleApp2;
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
    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
            new Product {Name = "Mamma Mia",  Description = "Film", Pris = 60, CategoryId = 2}
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
    // Seed Books
    if (!await db.Books.AnyAsync())
    {
        db.Books.AddRange(
            new Book {BookTitle = "Good Omens", ReleaseYear = 1970},
            new Book {BookTitle = "Star Wars, Return of the Jedi", ReleaseYear = 1977}
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
    
    // Seed Author
    if (!await db.Authors.AnyAsync())
    {
        db.Authors.AddRange(
            new Author {AuthorName = "Neil Gaiman", AuthorCountry = "England"},
            new Author {AuthorName = "Terry Pratchet", AuthorCountry = "England"},
            new Author {AuthorName = "George Lucas", AuthorCountry = "USA"}
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
}

// CLI för CRUD; CREATE, READ, UPDATE DELETE
while (true)
{
    Console.WriteLine("Meny");
    Console.WriteLine("1 = Categories");
    Console.WriteLine("2 = Products");
    Console.WriteLine("3 = Lib");
    Console.WriteLine("exit = avsluta");
    Console.WriteLine(">");

    
    var choice = Console.ReadLine()?.Trim();

    if (choice == "exit")
        break;

    if (choice == "1")
        await CategoryMenu();
    else if (choice == "2")
        await ProductMenu();
    else if (choice == "3")
        await LibMenu();
    else
        Console.WriteLine("Ogiltigt val.");

    static async Task LibMenu()
    {
        while (true)
        {
            Console.WriteLine("\nCommands: Auths | Books | Auth+ <id> | Book+ <id> | exit");
            Console.WriteLine(">");
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

            // Enkel switch för kommando
            switch (cmd)
            {
                case "auths":
                    await Auths();
                    break;
                case "books":
                    await Books();
                    break;
                case "auth+":
                    await AddAuths();
                    break;
                case "book+":
                    await AddBooks();
                    break;

            }


        }
    }

    static async Task CategoryMenu()
    {
        while(true)
        {
            Console.WriteLine("\nCommands: List | add | delete <id> | edit <id> | exit");
            Console.WriteLine(">");
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
                    // Kräver Id efter kommandot "edit"
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                    {
                        Console.WriteLine("Usage: Edit <id>");
                        break;
                    }
                    await EditAsync(id);
                    break;
                case "delete":
                    // Radera en category
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
                    {
                        Console.WriteLine("Usage: Delete <id>");
                        break;
                    }
                    await DeleteAsync(idD);
                    break;
                default:
                    Console.WriteLine($"Unknown command.");
                    break;
            
            }
        }
    }
    
    static async Task ProductMenu()
    {
        while(true)
        {
            Console.WriteLine("\nCommands: List | add | delete <id> | edit <id> | exit");
            Console.WriteLine(">");
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
                case "cl":
                    await ListAsync();
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var idCategory))
                    {
                        Console.WriteLine("Usage: Products By Category <id>");
                        break;
                    }
                    await ProductsByCategoryAsync(idCategory);
                    break;
                case "list":
                    // Lista våra categories
                    await ListProductsAsync();
                    break;
                case "add":
                    // Lägg till en category
                    await AddProductAsync();
                    break;
                case "edit":
                    // Redigera en category
                    // Kräver Id efter kommandot "edit"
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var EditId))
                    {
                        Console.WriteLine("Usage: Edit <id>");
                        break;
                    }
                    await EditProductAsync(EditId);
                    break;
                case "delete":
                    // Radera en category
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var DelitId))
                    {
                        Console.WriteLine("Usage: Delete <id>");
                        break;
                    }
                    await DeleteProductAsync(DelitId);
                    break;
                default:
                    Console.WriteLine($"Unknown command.");
                    break;
            
            }
        }
    }
}

static async Task AddBooks()
{
    Console.WriteLine("Available Authors:");
    await Auths();
    Console.WriteLine("Choose AuthorId:");
    var AIDInput = Console.ReadLine() ?? string.Empty;
    
    if (!int.TryParse(AIDInput, out var authorId))
    {
        Console.WriteLine("AuthorId must be a number.");
        return;
    }
    Console.WriteLine("Title:");
    var title = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrEmpty(title) || title.Length > 100)
    {
        Console.WriteLine("Title is required.");
        return;
    }
    
    Console.WriteLine("Year of realease:");
    if (!int.TryParse(Console.ReadLine(), out var year))
    {
        Console.WriteLine("Year must be a number.");
        return;
    }
    
    using var db = new ShopContext();
    await db.Books.AddAsync(new Book
    {
        BookTitle = title,
        ReleaseYear = year,
        AuthorId  = authorId
    });
    
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Book added.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("Db Error! " + ex.GetBaseException().Message);
    }
}





static async Task AddAuths()
{
    Console.WriteLine("Name");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrEmpty(name) || name.Length > 100)
    {
        Console.WriteLine("Name is required.");
        return;
    }
    
    Console.WriteLine("Country:");
    var country = Console.ReadLine()?.Trim() ?? string.Empty;
    
    using var db = new ShopContext();
    await db.Authors.AddAsync(new Author
    {
        AuthorName = name,
        AuthorCountry = country
    });
    
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Author added.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("Db Error! " + ex.GetBaseException().Message);
    }
}



static async Task Books()
{
    var db = new ShopContext();
    var rows = await db.Books.Include(book => book.Author).AsNoTracking().OrderBy(x => x.BookTitle).ToListAsync();
    Console.WriteLine("Id | Name | Release date | Author");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.BookId} | {row.BookTitle} | {row.ReleaseYear} | {row.Author?.AuthorName}");
    }
}


static async Task Auths()
{
    
    var db = new ShopContext();
    var rows = await db.Authors.AsNoTracking().OrderBy(x => x.AuthorName).ToListAsync();
    Console.WriteLine("Id | Name | Country ");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.AuthorId} | {row.AuthorName} | {row.AuthorCountry}");
    }
}



static async Task DeleteAsync(int id)
{
    using var db = new ShopContext();
    
    var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
    if (category == null)
    {
        Console.WriteLine("Category not found.");
        return;
    }
    db.Categories.Remove(category);
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Category deleted");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine(exception.Message);
    }
}

static async Task EditAsync(int id)
{
    using var db = new ShopContext();
    
    // Hämta raden vi vill uppdatera
    var category = await db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
    if (category == null)
    {
        Console.WriteLine("Category not found.");
        return;
    }
    
    // Visar nuvarande värden; Uppdatera namn för en specifik category
    Console.WriteLine($"{category.CategoryName} ");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(name))
    {
        category.CategoryName = name;
    }
    
    // Uppdaterar description för en specifik category; TODO: FIX ME (NULL, not required)
    Console.WriteLine($"{category.CategoryDescription} ");
    var description = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(description))
    {
        category.CategoryDescription = description;
    }

    // Uppdaterar 
    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Category edited");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine(exception.Message);
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

static async Task ProductsByCategoryAsync(int categoryId)
{
    using var db = new ShopContext();
    var products = await db.Products.Where(p => p.CategoryId == categoryId)
        .Include(p => p.Category)
        .OrderBy(p => (int)p.Pris).ToListAsync();
    
    Console.WriteLine("Id | Name | Description | Price | Category Name");
    foreach (var p in products)
    {
        Console.WriteLine($"{p.ProductId} {p.Name} {p.Description} {p.Pris} {p.Category?.CategoryName}");
    }
    
}

static async Task ListProductsAsync()
{
    using var db  = new ShopContext();
    
    var rows = await db.Products
        .Include(product => product.Category)
        .AsNoTracking()
        .OrderBy(p => p.Name)
        .ToListAsync();
    
    Console.WriteLine("Id | Name | Description | Pris | Category Name");
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.ProductId} | {row.Name} | {row.Description} | {row.Pris} | {row.Category?.CategoryName}");    
    }
}

static async Task AddProductAsync()
{
    Console.WriteLine("Name:");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrEmpty(name) || name.Length > 100)
    {
        Console.WriteLine("Name is required.");
        return;
    }

    Console.WriteLine("Description:");
    var desc = Console.ReadLine() ?? string.Empty;

    Console.WriteLine("Pris:");
    if (!decimal.TryParse(Console.ReadLine(), out var pris))
    {
        Console.WriteLine("Pris must be a number.");
        return;
    }
    Console.WriteLine("Available categories:");
    await ListAsync();
    Console.WriteLine("Choose CategoryId:");
    var CIDInput = Console.ReadLine()?.Trim() ?? string.Empty;
    
    if (!int.TryParse(CIDInput, out var categoryId))
    {
        Console.WriteLine("Category must be a number.");
        return;
    }


    using var db = new ShopContext();
    await db.Products.AddAsync(new Product
    {
        Name = name,
        Description = desc,
        Pris = pris,
        CategoryId  = categoryId
    });

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Product added.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("Db Error! " + ex.GetBaseException().Message);
    }
}

static async Task EditProductAsync(int id)
{
    using var db = new ShopContext();

    var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
    if (product == null)
    {
        Console.WriteLine("Product not found.");
        return;
    }

    Console.WriteLine($"Name ({product.Name}):");
    var name = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(name))
        product.Name = name;

    Console.WriteLine($"Description ({product.Description}):");
    var desc = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(desc))
        product.Description = desc;

    Console.WriteLine($"Pris ({product.Pris}):");
    var prisInput = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(prisInput) && decimal.TryParse(prisInput, out var pris))
        product.Pris = pris;

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Product updated.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.Message);
    }
}


static async Task DeleteProductAsync(int id)
{
    using var db = new ShopContext();

    var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
    if (product == null)
    {
        Console.WriteLine("Product not found.");
        return;
    }

    db.Products.Remove(product);

    try
    {
        await db.SaveChangesAsync();
        Console.WriteLine("Product deleted.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(ex.Message);
    }
}