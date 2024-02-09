using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GirlsWantTheBestShop.Models
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<Product>? Products { get; set; }

        public string CardHolderName { get; set; }
       
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CVV { get; set; }

        


    }
}
