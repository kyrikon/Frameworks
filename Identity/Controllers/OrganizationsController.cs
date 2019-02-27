using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLEXOS.Identity.Data;
using PLEXOS.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Core.Models;

namespace PLEXOS.Identity.Controllers
{
   
    [Authorize(Roles = "Admin")]
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Organizations
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Organizations.ToListAsync());
        }

       

        // GET: Organizations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organization = await _context.Organizations
                .SingleOrDefaultAsync(m => m.ID == id);
            if (Organization == null)
            {
                return NotFound();
            }
            Organization.Members = await (from clm in _context.UserClaims.Where(x => x.ClaimType.Equals("Organization", StringComparison.InvariantCultureIgnoreCase)
                                                            && x.ClaimValue.Equals(Organization.Name, StringComparison.InvariantCultureIgnoreCase))
                                            join usr in _context.Users on clm.UserId equals usr.Id
                                            select usr.UserName).ToListAsync();
            return View(Organization);
        }

        // GET: Organizations/Create
        public IActionResult Create()
        {
            Organization Organization = new Organization() { ID = Guid.NewGuid().ToString() };
            return View(Organization);
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description")] Organization Organization)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Organization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Organization);
        }

        // GET: Organizations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organization = await _context.Organizations.SingleOrDefaultAsync(m => m.ID == id);
            if (Organization == null)
            {
                return NotFound();
            }
            return View(Organization);
        }

        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Name,Description")] Organization Organization)
        {
            if (id != Organization.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Organization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(Organization.ID))
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
            return View(Organization);
        }

        // GET: Organizations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organization = await _context.Organizations
                .SingleOrDefaultAsync(m => m.ID == id);
            if (Organization == null)
            {
                return NotFound();
            }

            return View(Organization);
        }

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var Organization = await _context.Organizations.SingleOrDefaultAsync(m => m.ID == id);
            _context.Organizations.Remove(Organization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizationExists(string id)
        {
            return _context.Organizations.Any(e => e.ID == id);
        }
    }
}
