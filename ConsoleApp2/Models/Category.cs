using System.ComponentModel.DataAnnotations;

namespace ConsoleApp2.Models;

// Enkel modelklass (enetity) som EF core mappa till en tabell "Category"
public class Category
{
    //primärnyckel. EF cOre ser "CategoryID" och gör den till PK
    public int CategoryId { get; set; }
    
    //Required = får inte vara null (Varken i C# eller i databasen)
    // MaxLength = genererar en kolumn med maxlängd 100 + används vid validering
    [Required, MaxLength(100)]
    public string CategoryName { get; set; } = null!;
    
    // Optional (Nullable '?') text med max 250
    [MaxLength(250)]
    public string? CategoryDescription { get; set; }
 
    // En lista av produkterna
    public List<Product> Products { get; set; } = new();

}