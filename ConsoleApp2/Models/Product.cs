using System.ComponentModel.DataAnnotations;

namespace ConsoleApp2.Models;

public class Product
{
    public int ProductId { get; set; }
    [Required]
    public decimal Pris {get ; set;}
    [Required, MaxLength(100)]
    public string Name {get; set;}
    [MaxLength(250)]
    public string? Description {get; set;}
    
    // Foreign Key
    public int CategoryId { get; set; }

    // Navigation
    public Category Category { get; set; } = null!;

}