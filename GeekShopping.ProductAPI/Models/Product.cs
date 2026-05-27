using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeekShopping.ProductAPI.Models.Base;

namespace GeekShopping.ProductAPI.Models;

[Table("product")]
public class Product : BaseEntity
{
    //a propriedade Name não pode ser anulável, usando C# com nullable reference types habilitado o compilador está avisando que: 
    // "A propriedade Name precisa conter um valor não-nulo ao sair do construtor"
    //Sendo assim foi adicionado o operador de nulabilidade "?" para indicar que a propriedade pode ser nula, 
    // e também foi adicionado o atributo [Required] para garantir que o valor seja fornecido ao criar ou atualizar um produto.  
    [Column("name")]
    [Required]
    [StringLength(150)]
    public string? Name { get; set; }

    [Column("price")]
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [Column("category_name")]
    [StringLength(100)]
    public string? CategoryName { get; set; }

    [Column("image_url")]
    [StringLength(300)]
    public string? ImageUrl { get; set; }
}
