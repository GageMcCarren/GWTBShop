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
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SpecialTags
        [Route("Admin/SpecialTags/Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.SpecialTag.ToListAsync());
        }

        // GET: Admin/SpecialTags/Details/5
        [Route("Admin/SpecialTags/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTag
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        // GET: Admin/SpecialTags/Create
        [Authorize(Roles = "Admin")]
        [Route("Admin/SpecialTags/Create")]
        public IActionResult Create()
        {
            return View("Areas/Admin/Views/SpecialTags/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialTag);
        }

        // GET: Admin/SpecialTags/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/SpecialTags/Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTag.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        // POST: Admin/SpecialTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SpecialTag specialTag)
        {
            if (id != specialTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialTagExists(specialTag.Id))
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
            return View(specialTag);
        }

        // GET: Admin/SpecialTags/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Admin/SpecialTags/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTag
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        // POST: Admin/SpecialTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialTag = await _context.SpecialTag.FindAsync(id);
            if (specialTag != null)
            {
                _context.SpecialTag.Remove(specialTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialTagExists(int id)
        {
            return _context.SpecialTag.Any(e => e.Id == id);
        }
    }
}
