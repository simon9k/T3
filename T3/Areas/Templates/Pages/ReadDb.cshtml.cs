using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using T3.Areas.Templates.Data;
using T3.Data;

#region 这段程序中有如何使用从DB中获取excel文件，并再打开的过程
#endregion
namespace T3.Areas.Templates.Pages
{
    public class ReadDbModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReadDbModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [ViewData]
        public string Message { get; set; }
        public bool bPicture { get; set; }


        [BindProperty]
        public Template ReadFile { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            ReadFile = await _context.Templates.SingleOrDefaultAsync(m => m.Id == id);
            bPicture = ReadFile.UntrustedName.Contains(".png")|| ReadFile.UntrustedName.Contains(".jpg");
            //bPicture = Regex.IsMatch(ReadFile.UntrustedName,".png ")
            //if picture
            //transfer binary data to base64string so that can be transfered together! and reduce another request.
            //            ViewBag.Image = ViewImage(bytes);

       

            //private string ViewImage(byte[] arrayImage)

            //{

            //    string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);

            //    return "data:image/png;base64," + base64String;

            //}
            if (bPicture)
            { 
                if (ReadFile.UntrustedName.Contains(".png"))
                    Message = "data:image/png;base64," + Convert.ToBase64String(ReadFile.Content, 0, ReadFile.Content.Length);
                else
                    Message = "data:image/jpg;base64," + Convert.ToBase64String(ReadFile.Content, 0, ReadFile.Content.Length);

            }
            else
            {
                //if excel
                using (var dataStream = new MemoryStream())
                {

                    //Copy data of file from DB
                    dataStream.Write(ReadFile.Content);


                    using (var reader = ExcelReaderFactory.CreateReader(dataStream))
                    {
                        var result = reader.AsDataSet(
                          new ExcelDataSetConfiguration()
                          {
                          // Gets or sets a callback to obtain configuration options for a DataTable. 
                          ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                              {
                              // Gets or sets a value indicating whether to use a row from the 
                              // data as column names.
                              //第一行为标题列
                              UseHeaderRow = true,
                              }
                          });

                        //todo 不同版本excel文件的测试
                        Message = "";
                        //遍历每一张sheet，每个sheet对应一个entity类（如Pupil、PupilLog），即将初始化的数据
                        foreach (DataTable p in result.Tables)
                        {
                            Message += p.TableName + " ";

                            ////TableName是excel的sheet的名称，根据不同名称对应不同的实体entity
                            //switch (p.TableName)
                            //{
                            //    case "宝贝":   //seeding Pupil，给出指定需要赋值的属性
                            //        EntityProperities = new List<string> { "Name", "NickName", "Sex", "EnrollmentDate", "ClassID" };
                            //        type = typeof(Pupil);
                            //        break;
                            //}
                        }

                        //await file.CopyToAsync(dataStream);
                        //fileModel.Data = dataStream.ToArray();
                    }

                }
            }

            //File file = new File();
            //ReadFile.Content;
            if (ReadFile == null)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            ReadFile = await _context.Templates.FindAsync(id);

            if (ReadFile != null)
            {
                _context.Templates.Remove(ReadFile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }
    }
}

