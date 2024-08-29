using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model { }

public class ProductModel
{
    public long ProductId { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Price is required!")]
    [DataType("decimal(18,2)")]
    public decimal Price { get; set; }

    public string ImageBase64String { get; set; }

    [Required(ErrorMessage = "CategoryId is required!")]
    public long CategoryId { get; set; }

    public long CreatedBy { get; set; }
}
