using System.ComponentModel.DataAnnotations;

namespace Model { }

public class UserModel
{
    public long UserId { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required!")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Email is required!")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Email is Invalid!")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required!")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Created By is required!")]
    public long CreatedBy { get; set; }
}
