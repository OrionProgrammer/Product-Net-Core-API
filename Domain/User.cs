using System.ComponentModel.DataAnnotations;

namespace Domain { }

public record User
{
    [Key]
    public required long UserId { get; set; }

    //concurrency check to make sure 2 updates don't conflict
    [ConcurrencyCheck]
    public required string Name { get; set; }

    [ConcurrencyCheck]
    public required string Surname { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required DateTime CreatedDate { get; set; } = DateTime.Now;
}
