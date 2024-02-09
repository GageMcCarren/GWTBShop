using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GirlsWantTheBestShop.Data;
using GirlsWantTheBestShop.Models;
using System.Configuration.Internal;
using Microsoft.AspNetCore.Authorization;


namespace GirlsWantTheBestShop.Areas.Admin.Controllers
{
    
        [Area("Admin")]
        [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webH;

       
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webH)
        {
            _context = context;
            _webH = webH;
        }

        // GET: Admin/Products
        [Route("Admin/Products/Index")]
        public IActionResult Index()
        {
            return View(_context.Products.Include(c => c.ProductTypes).Include(f => f.SpecialTag).ToList());
        }

        //POST Index action method
        [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? largeAmount)
        {
            var products = _context.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .Where(c => c.Price >= lowAmount && c.Price <= largeAmount).ToList();

            if (lowAmount == null || largeAmount == null)
            {
                products = _context.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList();
            }
            return View(products);
        }

        //Get Create method
        [Authorize(Roles = "Admin")]
        [Route("Admin/Products/Create")]
        public IActionResult Create() { 
        
        
            ViewData["productTypeId"] = new SelectList(_context.ProductTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(_context.SpecialTag.ToList(), "Id", "Name");
            return View("~/Areas/Admin/Views/Products/Create.cshtml");
        }


        //Post Create method
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile image)
          {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); 
                }
            }


            if (ModelState.IsValid)
            {
                var searchProduct = _context.Products.FirstOrDefault(c => c.Name == product.Name);
                if (searchProduct != null)
                {
                    ViewBag.message = "This product is already exist";
                    ViewData["productTypeId"] = new SelectList(_context.ProductTypes.ToList(), "Id", "ProductType");
                    ViewData["TagId"] = new SelectList(_context.SpecialTag.ToList(), "Id", "Name");
                    return View(product);
                }

                if (image != null)
                {
                    string uniqueFileName = Path.GetFileNameWithoutExtension(image.FileName)
                           + "_"
                           + Guid.NewGuid().ToString()
                           + Path.GetExtension(image.FileName);

                    var filePath = Path.Combine(_webH.WebRootPath + "/Images", uniqueFileName);
                    await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    product.Image = "Images/ProductImage/" + uniqueFileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("~/Areas/Admin/Views/Products/Index.cshtml");
            }

            return View(product);
        }

        //GET Edit Action Method
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            ViewData["productTypeId"] = new SelectList(_context.ProductTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(_context.SpecialTag.ToList(), "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST Edit Action Method
        [HttpPost]
        public async Task<IActionResult> Edit(Product products, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string uniqueFileName = Path.GetFileNameWithoutExtension(image.FileName)
                           + "_"
                           + Guid.NewGuid().ToString()
                           + Path.GetExtension(image.FileName);

                    var filePath = Path.Combine(_webH.WebRootPath + "/Images/ProductImage/", uniqueFileName);
                    await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    products.Image = "Images/ProductImage/" + uniqueFileName;
                }

                if (image == null)
                {
                    products.Image = "Images/No-Image-Placeholder.svg.png";
                }
                _context.Products.Update(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        //GET Details Action Method
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //GET Delete Action Method
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.SpecialTag).Include(c => c.ProductTypes).Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST Delete Action Method

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
