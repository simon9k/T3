using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using T3.Areas.Templates.Data;
using T3.Data;

namespace T3.Areas.Templates.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        //private readonly IFileProvider _fileProvider;

        public IndexModel(ApplicationDbContext context/*, IFileProvider fileProvider*/)
        {
            _context = context;
            //_fileProvider = fileProvider;
            context.Database.EnsureCreated();//This is not a good place to do this

        }

        public IList<Template> DatabaseFiles { get; private set; }
        //public IDirectoryContents PhysicalFiles { get; private set; }

        public async Task OnGetAsync()
        {
            DatabaseFiles = await _context.Templates.AsNoTracking().ToListAsync();
            //PhysicalFiles = _fileProvider.GetDirectoryContents(string.Empty);
        }

        public async Task<IActionResult> OnGetDownloadDbAsync(int? id)
        {
            if (id == null)
            {
                return Page();
            }

            var requestFile = await _context.Templates.SingleOrDefaultAsync(m => m.Id == id);

            if (requestFile == null)
            {
                return Page();
            }

            // Don't display the untrusted file name in the UI. HTML-encode the value.
            return File(requestFile.Content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(requestFile.UntrustedName));
        }

        //public IActionResult OnGetDownloadPhysical(string fileName)
        //{
        //    var downloadFile = _fileProvider.GetFileInfo(fileName);

        //    return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, fileName);
        //}
    }
}