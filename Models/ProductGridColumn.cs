using System.ComponentModel.DataAnnotations;

namespace c1Soft_Projesi.Models;

public class ProductGridColumn
{
    public int ProductGridColumnId { get; set; }

    [Required]
    [StringLength(50)]
    public string FieldName { get; set; } = "";

    [Required]
    [StringLength(100)]
    public string DisplayName { get; set; } = "";

    public int SortOrder { get; set; }

    [Required]
    [StringLength(30)]
    public string RenderType { get; set; } = "Text";

    public int ColumnWidth { get; set; } = 120;

    public bool IsVisible { get; set; } = true;

    public bool ShowOnMobile { get; set; } = true;
}
