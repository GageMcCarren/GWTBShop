using GirlsWantTheBestShop.Data;
using GirlsWantTheBestShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Data.SqlTypes;

namespace GirlsWantTheBestShop.Areas.Customer.Controllers
{


    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _context;  
        private readonly IEmailSender _emailSender;

        public OrderController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }   

        //CheckOut

        public IActionResult CheckOut()
        {
            var viewModel = new CheckoutViewModel
            {
                Order = new Order(),
                Products = HttpContext.Session.Get<List<Product>>("products")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
          


            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products != null)
            {
                foreach (var product in products)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.ProductId = product.Id;
                    viewModel.Products = HttpContext.Session.Get<List<Product>>("products"); 
                   
                    viewModel.Order.OrderDetails.Add(orderDetails);
                }
            }

           

            viewModel.Order.OrderNo = GetOrderNo();
            _context.Orders.Add(viewModel.Order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Set("products", new List<Product>());

           /* var productDetails = viewModel.Products.Select(p => 
            $"Product Name: (p.Name), Price: (p.Price)").ToList();

            string productList = string.Join(",\n", productDetails);
            string emailContent = $"Order Number: {viewModel.Order.OrderNo}\n" +
                $"Customer Name: {viewModel.Order.Name}\n" +
                $"Products: \n{productList}";

            await _emailSender.SendEmailAsync("girlswantthebestshop@gmail.com", "New Order Recieved", emailContent); */


            return View();
        }

      


      

        public IActionResult OrderConfirmation()
        {
            
            return View();
        }

        public string GetOrderNo()
        {
            int rowCount = _context.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }

    }

      
    
}
