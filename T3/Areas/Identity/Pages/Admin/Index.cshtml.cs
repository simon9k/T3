using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using T3.Areas.Identity.Data;

namespace T3.Areas.Identity.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly AppUserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            AppUserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IList<AppUser> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await Task.Run(()=> _userManager.Users.ToList());
            if (Users == null)
            {
                return NotFound($"Unable to load users.");
            }

            return Page();
        }
    }
}
