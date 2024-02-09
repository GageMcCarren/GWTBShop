using GirlsWantTheBestShop.Models;

namespace GirlsWantTheBestShop
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Product> GasProducts { get; set; }
        public IEnumerable<Product> DieselProducts { get; set; }

    }
}
