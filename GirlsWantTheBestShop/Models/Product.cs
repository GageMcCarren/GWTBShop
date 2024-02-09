using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GirlsWantTheBestShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }

        public string? Image { get; set; } = string.Empty;

        public string? Description { get; set; }

      

        [Display(Name = "Is A Package Item")]
        public bool IsPackageItem { get; set; }

        public bool IsDiesel {  get; set; }

        public bool IsGas {  get; set; }

        public int Quantity { get; set; }


        [Display(Name = "Product Type")]
        [Required]
        public int ProductTypeId { get; set; }
        [ForeignKey("ProductTypeId")]
        public virtual ProductTypes? ProductTypes { get; set; }
        [Display(Name = "Special Tag")]
        [Required]
        public int SpecialTagId { get; set; }
        [ForeignKey("SpecialTagId")]
        public virtual SpecialTag? SpecialTag { get; set; }
    }
}



