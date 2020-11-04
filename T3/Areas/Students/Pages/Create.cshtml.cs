using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using T3.Data;
using T3.Models;

namespace T3.Areas.Students.Pages
{
    public class CreateModel : PageModel
    {
        private readonly T3.Data.ApplicationDbContext _context;
        private readonly ITenantResolver _tenantResolver;

        public CreateModel(T3.Data.ApplicationDbContext context, ITenantResolver tenantResolver)
        {
            _tenantResolver = tenantResolver;
            _context = context;

            //todo should jnject Tenant here
        }

        public IActionResult OnGet()
        {
        ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId");
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Student.TenantId = _tenantResolver.GetTenantId();

            _context.Students.Add(Student);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
