using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GirlsWantTheBestShop.Data;
using GirlsWantTheBestShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace GirlsWantTheBestShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductTypes
        [Route("Admin/ProductTypes/Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductTypes.ToListAsync());
        }

        // GET: Admin/ProductTypes/Details/5
        [Route("Admin/ProductTypes/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTypes = await _context.ProductTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productTypes == null)
            {
                return NotFound();
            }

            return View(productTypes);
        }

        // GET: Admin/ProductTypes/Create
        [Authorize(Roles = "Admin")]
        [Route("Admin/ProductTypes/Create")]
        public IActionResult Create()
        {
            return View("~/Areas/Admin/Views/ProductTypes/Create.cshtml");
        }

        // POST: Admin/ProductTypes/Create
        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductType")] ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productTypes);
                await _context.SaveChangesAsync();
                TempData["save"] = "Product Type has been Saved"; 
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }









        // GET: Admin/ProductTypes/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/ProductTypes/Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTypes = await _context.ProductTypes.FindAsync(id);
            if (productTypes == null)
            {
                return NotFound();
            }
            return View(productTypes);
        }

        // POST: Admin/ProductTypes/Edit/5
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductType")] ProductTypes productTypes)
        {
            if (id != productTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productTypes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductTypesExists(productTypes.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }









        // GET: Admin/ProductTypes/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/ProductTypes/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTypes = await _context.ProductTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productTypes == null)
            {
                return NotFound();
            }

            return View(productTypes);
        }

        // POST: Admin/ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productTypes = await _context.ProductTypes.FindAsync(id);
            if (productTypes != null)
            {
                _context.ProductTypes.Remove(productTypes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTypesExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }
    }
}
