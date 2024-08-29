using System.ComponentModel.DataAnnotations;

namespace Model { }

public class UserCategoryModel
{
    [Required(ErrorMessage = "User Id is required!")]
    public long UserId { get; set; }

    [Required(ErrorMessage = "Category Id is required!")]
    public long CategoryId { get; set; }

}
