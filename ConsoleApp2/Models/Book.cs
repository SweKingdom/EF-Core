using System.ComponentModel.DataAnnotations;

namespace ConsoleApp2.Models;

public class Book
{
    //Primärnyckel
    public int BookId { get; set; }
    [Required, MaxLength(100)]
    public string BookTitle { get; set; } = null!;
    [MaxLength(4)]
    public int? ReleaseYear { get; set; }

    public List<Author> Authors { get; set; } = new();
}