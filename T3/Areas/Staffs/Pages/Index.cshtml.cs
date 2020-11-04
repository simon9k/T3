using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using T3.Data;
using T3.Models;

namespace T3.Areas.Staffs.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly T3.Data.ApplicationDbContext _context;
        private readonly StaffManager _staffManager;

        public IndexModel(StaffManager staffManager, ITenantResolver tenantResolver)
        {
            _staffManager = staffManager;
        }

        public IList<Staff> Staff { get;set; }

        public async Task OnGetAsync()
        {
            Staff = await _staffManager.GetAllAsync();
            //Staff = await _context.Staffs
            //    .Include(s => s.Tenant).ToListAsync();
        }
    }
}
