using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KPIASPNETCore.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KPIASPNETCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Produces("application/json")]
    public class FileUploadController : ControllerBase // IHostingEnvironment env
    {
        IHostingEnvironment _env;
        string imagefolder = "imagestest";
        string jsonfile = "records.json";

        public FileUploadController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost("Uploadfile")]
        public async Task<IActionResult> Post(string name, List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var folderPath = TestDirectory();

            var records = ParseRecords();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    if (records.Where(r => r.FileName == formFile.FileName).Count() == 0)
                    {
                        Imagedto record = new Imagedto() { Id = records.Count, Author = name, FileName = formFile.FileName, Rate = 0 };
                        records.Add(record);
                        using (var stream = new FileStream(Path.Combine(folderPath, formFile.FileName), FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }
            await System.IO.File.WriteAllTextAsync(Path.Combine(_env.WebRootPath, jsonfile), JsonConvert.SerializeObject(records));

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, folderPath });
        }

        [HttpGet("GetImages")]
        public JsonResult GetImages()
        {
            var records = ParseRecords();

            //List<string> files = new List<string>();
            //string[] fileEntries = Directory.GetFiles(folderPath);
            //foreach (var file in fileEntries)
            //{
            //    files.Add((Path.Combine("/" + imagefolder, Path.GetFileName(file))).Replace('\\', '/'));
            //}

            var result = new JsonResult(records);
            //var result = records.Select(r => r.Id.ToString() + "," + r.FileName + "," + r.Author + "," + r.Rate.ToString());

            return result; 
        }

        [HttpGet("Deletefile")]
        public async Task<IActionResult> GetDelete(string fileName)
        {
            var folderPath = Path.Combine(_env.WebRootPath, imagefolder);
            var filename = Path.GetFileName(fileName);
            var records = ParseRecords();

            if (!string.IsNullOrEmpty(filename))
            {
                var fullName = Path.Combine(folderPath, filename);
                if (System.IO.File.Exists(fullName))
                {
                    System.IO.File.Delete(fullName);
                }
                var record = records.Where(r => r.FileName == filename).FirstOrDefault();
                if(records.Remove(record))
                {
                    await System.IO.File.WriteAllTextAsync(Path.Combine(_env.WebRootPath, jsonfile), JsonConvert.SerializeObject(records));
                    return Ok();
                }
                return NotFound();
            }

            return NotFound();
        }

        [HttpGet("SetRate")]
        public async Task<IActionResult> GetSetRate(int id, int rate)
        {
            var records = ParseRecords();
            var record = records.Where(r => r.Id == id).FirstOrDefault();
            record.Rate = rate;

            await System.IO.File.WriteAllTextAsync(Path.Combine(_env.WebRootPath, jsonfile), JsonConvert.SerializeObject(records));

            return Ok();
        }

        public List<Imagedto> ParseRecords()
        {
            var records = new List<Imagedto>();
            var jsonfilepath = Path.Combine(_env.WebRootPath, jsonfile);

            if (System.IO.File.Exists(jsonfilepath))
            {
                records = JsonConvert.DeserializeObject<List<Imagedto>>(System.IO.File.ReadAllText(jsonfilepath));
            }
            return records;
        }

        public string TestDirectory()
        {
            var folderPath = Path.Combine(_env.WebRootPath, imagefolder); //Path.GetTempFileName();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }
    }
}