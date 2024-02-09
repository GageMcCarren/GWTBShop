using GirlsWantTheBestShop.Data;
using GirlsWantTheBestShop.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace GirlsWantTheBestShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;
      

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            
        }

        public IActionResult Index()
        {
            var gasProducts = _context.Products
     .Where(p => p.IsGas) 
     .Take(3)
     .Include(c => c.ProductTypes)
     .Include(c => c.SpecialTag)
     .ToList();

            var dieselProducts = _context.Products
                .Where(p => p.IsDiesel) 
                .Take(3)
                .Include(c => c.ProductTypes)
                .Include(c => c.SpecialTag)
                .ToList();

            var viewModel = new HomeIndexViewModel 
            {
                GasProducts = gasProducts,
                DieselProducts = dieselProducts
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //GET Product Detail Action Method

        public ActionResult Detail(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Include(c => c.ProductTypes).FirstOrDefault(c=>c.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        //Post Product Detail Action Method
        [HttpPost]
        [ActionName("Detail")]
        public ActionResult ProductDetail(int? id)
        {
            

            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            string serializedProducts = HttpContext.Session.GetString("products");
            List<Product> products;

            if (!string.IsNullOrEmpty(serializedProducts))
            {
                products = JsonSerializer.Deserialize<List<Product>>(serializedProducts);
            }
            else
            {
                products = new List<Product>();
            }
            products.Add(product);
            serializedProducts = JsonSerializer.Serialize(products);
            HttpContext.Session.SetString("products", serializedProducts);
            return View(product);
        }


        [ActionName("Remove")]
        public IActionResult RemoveFromCart(int? id)
        {
            string serializedProducts = HttpContext.Session.GetString("products");
            List<Product> products;
            if (!string.IsNullOrEmpty(serializedProducts))
            {
                products = JsonSerializer.Deserialize<List<Product>>(serializedProducts);
            }
            else
            {
                products = new List<Product>();
            }

            if (products != null && id != null)
            {
                // Find the product in the list
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    // Remove the product from the list
                    products.Remove(product);

                    // Serialize the updated list back to a JSON string
                    serializedProducts = JsonSerializer.Serialize(products);

                    // Update the session with the new list of products
                    HttpContext.Session.SetString("products", serializedProducts);
                }
            }


            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Remove(int? id)
        {
            string serializedProducts = HttpContext.Session.GetString("products");
            List<Product> products;

            // Deserialize the JSON string back to a list of products if it exists
            if (!string.IsNullOrEmpty(serializedProducts))
            {
                products = JsonSerializer.Deserialize<List<Product>>(serializedProducts);
            }
            else
            {
                products = new List<Product>();
            }

            // Check if the product list is not null and the id is valid
            if (products != null && id != null)
            {
                // Find the product in the list
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    // Remove the product from the list
                    products.Remove(product);

                    // Serialize the updated list back to a JSON string
                    serializedProducts = JsonSerializer.Serialize(products);

                    // Update the session with the new list of products
                    HttpContext.Session.SetString("products", serializedProducts);
                }
            }

            return RedirectToAction(nameof(Index));
        }


        //Get Product CART Action Method

        public IActionResult Cart()
        {
      
            string serializedProducts = HttpContext.Session.GetString("products");
            List<Product> products;

            if (!string.IsNullOrEmpty(serializedProducts))
            {
              
                products = JsonSerializer.Deserialize<List<Product>>(serializedProducts);
            }
            else
            {
               
                products = new List<Product>();
            }

            // Pass the list of products to the view
            return View(products);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ProductPage(int? page)
        {
            return View(_context.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            return await UpdateQuantity(id, 1);
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            return await UpdateQuantity(id, -1);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int newQuantity)
        {
            // Retrieve the list of products from the session
            string serializedProducts = HttpContext.Session.GetString("products");
            List<Product> products = !string.IsNullOrEmpty(serializedProducts)
                ? JsonSerializer.Deserialize<List<Product>>(serializedProducts)
                : new List<Product>();

            
            Product productToUpdate = products.FirstOrDefault(p => p.Id == id);
            if (productToUpdate != null)
            {
                
                productToUpdate.Quantity = Math.Max(newQuantity, 1);  

                
                serializedProducts = JsonSerializer.Serialize(products);
                HttpContext.Session.SetString("products", serializedProducts);

                
                return RedirectToAction(nameof(Cart));
            }

            
            return RedirectToAction(nameof(Cart));
        }





    }
}
