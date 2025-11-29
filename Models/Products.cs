using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{

    [Table("Prod_")] public class Product
    {
        [Key] // Первинний ключ
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва товару є обов'язковою")]
        [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів")]
        [Display(Name = "Назва товару")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Опис товару є обов'язковим")]
        [StringLength(1000, ErrorMessage = "Опис не може перевищувати 1000 символів")]
        [Display(Name = "Опис товару")]
        public string Description { get; set; }

        [Display(Name = "Зображення")]
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
    [Table("ProdImg_")] public class ProductImage
    {
        [Key] // Первинний ключ
        public int Id { get; set; }

        [Required(ErrorMessage = "Посилання на зображення є обов'язковим")]
        [Url(ErrorMessage = "Введіть коректний URL")]
        [Display(Name = "URL зображення")]
        public string Url { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }


}
