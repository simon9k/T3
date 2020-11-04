using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using T3.Models;

namespace T3.Areas.Staffs.Pages
{
    public class CreateModel : PageModel
    {
        //private readonly T3.Data.ApplicationDbContext _context;
        private readonly StaffManager _staffManager;

        public CreateModel(StaffManager staffManager)
        {
            _staffManager = staffManager;
        }

        
        [BindProperty]
        public Staff Staff { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //_context.Staffs.Add(Staff);
            //await _context.SaveChangesAsync();
            await _staffManager.AddAsync(Staff);

            return RedirectToPage("./Index");
        }
    }
}
