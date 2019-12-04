using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using T3.Areas.Identity.Data;
using T3.Data;
using T3.Models;

namespace T3.Controllers
{
    public class TenantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppUserManager<AppUser> _appUserManager;
        public TenantsController(ApplicationDbContext context,AppUserManager<AppUser> appUserManager)
        {
            _context = context;
            _appUserManager = appUserManager;

        }

        // GET: Tenants
        public async Task<IActionResult> Index()
        {
            _context.Tenants.IgnoreQueryFilters();
            return View(await _context.Tenants.IgnoreQueryFilters().ToListAsync());

            //return View(await _context.Tenants.ToListAsync());
        }

        // GET: Tenants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(m => m.TenantId == id);
            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        // GET: Tenants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tenants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantId,Name,Location,Desctiption")] Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                tenant.TenantId = Guid.NewGuid();
                //tenant.TenantId = Guid.Empty; //it does work, the TenantId changed by EF
                _context.Add(tenant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tenant);
        }

        // GET: Tenants/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        // POST: Tenants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TenantId,Name,Location,Desctiption")] Tenant tenant)
        {
            if (id != tenant.TenantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tenant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.TenantId))
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
            return View(tenant);
        }

        // GET: Tenants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(m => m.TenantId == id);
            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TenantExists(Guid id)
        {
            return _context.Tenants.Any(e => e.TenantId == id);
        }

        public async Task<IActionResult> RemoveAll()
        {
            //remove Global filter firstly
            //removing Tenants can automately remove AppUser cause of cascading delete
            //_context.Tenants.RemoveRange(_context.Tenants.IgnoreQueryFilters());
            //await _context.SaveChangesAsync();
            
            //Create instance through DI but not construction Injection!
            IConfiguration config = (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));
            Guid rootTenantId = config.GetValue<Guid>("RootTenantId");
            await DbInitializer.Initialize(_context, _appUserManager,rootTenantId);

            return RedirectToAction(nameof(Index));

        }
    }
}
