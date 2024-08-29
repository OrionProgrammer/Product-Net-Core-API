using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain { }

public record Product
{
    [Key]
    public required long ProductId { get; set; }

    [ConcurrencyCheck]
    public required string Name { get; set; }

    [ConcurrencyCheck]
    public required string Code { get; set; }

    public string Description { get; set; }

    [Precision(18, 2)]
    public required decimal Price { get; set; }

    public string ImageType { get; set; }

    public byte[] Image { get; set; }

    public required long CategoryId { get; set; }

    public required DateTime CreatedDate { get; set; } = DateTime.Now;

    public required long CreatedBy { get; set; }
}
