using System.ComponentModel.DataAnnotations;

namespace Model { }

public class CategoryModel
{
    public long CategoryId { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Code is required!")]
    public string Code { get; set; }

    [Required(ErrorMessage = "IsActive is required!")]
    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }
}
