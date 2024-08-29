using System.ComponentModel.DataAnnotations;

namespace Domain { }

public record UserCategory
{
    public UserCategory() { }

    [Key]
    public long UserCategoryId { get; set; }

    public required long UserId { get; set; }

    public required long CategoryId { get; set; }
}
