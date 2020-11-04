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

#region ��γ����������ʹ�ô�DB�л�ȡexcel�ļ������ٴ򿪵Ĺ���
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
                              //��һ��Ϊ������
                              UseHeaderRow = true,
                              }
                          });

                        //todo ��ͬ�汾excel�ļ��Ĳ���
                        Message = "";
                        //����ÿһ��sheet��ÿ��sheet��Ӧһ��entity�ࣨ��Pupil��PupilLog����������ʼ��������
                        foreach (DataTable p in result.Tables)
                        {
                            Message += p.TableName + " ";

                            ////TableName��excel��sheet�����ƣ����ݲ�ͬ���ƶ�Ӧ��ͬ��ʵ��entity
                            //switch (p.TableName)
                            //{
                            //    case "����":   //seeding Pupil������ָ����Ҫ��ֵ������
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

