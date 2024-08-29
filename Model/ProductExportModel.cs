using System.ComponentModel.DataAnnotations;

namespace Model { }

public class ProductExportModel
{
    public long ProductId { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    [DataType("decimal(18,2)")]
    public decimal Price { get; set; }

    public long CategoryId { get; set; }
}
