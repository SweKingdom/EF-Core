using System.ComponentModel.DataAnnotations;

namespace ConsoleApp2.Models;

public class Author
{
        
    //Primärnyckel
    public int AuthorId { get; set; }

    [Required, MaxLength(100)]
    public string AuthorName { get; set; } = null!;

    [MaxLength(100)] 
    public string? AuthorCountry { get; set; }
    
    public List<Book> Books { get; set; } = new();
}