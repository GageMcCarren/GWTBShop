using System.ComponentModel.DataAnnotations;

namespace GirlsWantTheBestShop.Models
{
    public class Order
    {
        public Order() 
        {
            OrderDetails = new List<OrderDetails>();

        }  


        public int Id { get; set; }
        [Display(Name = "Order Number")]
        public string OrderNo { get; set; }
        [Required]
        public string Name { get; set; }
      
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNo { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        public virtual List<OrderDetails> OrderDetails { get; set; }    

    }
}
