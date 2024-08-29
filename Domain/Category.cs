using System.ComponentModel.DataAnnotations;

namespace Domain { }

public record Category
{
    [Key]
    public required long CategoryId { get; set; }

    [ConcurrencyCheck]
    public required string Name { get; set; }

    [ConcurrencyCheck]
    public required string Code { get; set; }

    public required bool IsActive { get; set; }

    public required DateTime CreatedDate{ get; set; } = DateTime.Now;

    public required long CreatedBy { get; set; }

}
