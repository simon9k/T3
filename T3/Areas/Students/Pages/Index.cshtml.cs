using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using T3.Data;
using T3.Models;

namespace T3.Areas.Students.Pages
{
    public class IndexModel : PageModel
    {
        private readonly T3.Data.ApplicationDbContext _context;

        public IndexModel(T3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Student> Student { get;set; }

        public async Task OnGetAsync()
        {
            Student = await _context.Students
                .ToListAsync();
                //.Include(s => s.Tenant).ToListAsync();
        }
    }
}
