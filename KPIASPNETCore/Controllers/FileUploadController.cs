using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KPIASPNETCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Produces("application/json")]
    public class FileUploadController : ControllerBase // IHostingEnvironment env
    {
        IHostingEnvironment _env;
        string imagefolder = "imagestest";

        public FileUploadController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost("Uploadfile")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var folderPath = Path.Combine(_env.WebRootPath, imagefolder); //Path.GetTempFileName();
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(Path.Combine(folderPath, formFile.FileName), FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, folderPath });
        }

        [HttpGet("GetImages")]
        public ActionResult<IEnumerable<string>> GetImages()
        {
            var folderPath = Path.Combine(_env.WebRootPath, imagefolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            List<string> files = new List<string>();
            string[] fileEntries = Directory.GetFiles(folderPath);
            foreach(var file in fileEntries)
            {
                files.Add((Path.Combine("/" + imagefolder, Path.GetFileName(file))).Replace('\\', '/'));
            }
            return files;
        }

        //[HttpPost("Deletefile")]
        //public IActionResult PostDelete(List<IFormFile> files)
        //{
        //    long size = files.Sum(f => f.Length);
        //    var folderPath = Path.Combine(_env.WebRootPath, "images");

        //    //if(!string.IsNullOrEmpty(file))
        //    //{
        //    //    var fullName = Path.Combine(folderPath, file);
        //    //    if (System.IO.File.Exists(fullName))
        //    //    {
        //    //        System.IO.File.Delete(fullName);
        //    //    }
        //    //}

        //    foreach (var formFile in files)
        //    {
        //        var fullName = Path.Combine(folderPath, formFile.FileName);
        //        if (System.IO.File.Exists(fullName))
        //        {
        //            System.IO.File.Delete(fullName);
        //        }
        //    }

        //    return Ok(new { deleted = "deleted" });
        //}

        [HttpGet("Deletefile")]
        public IActionResult GetDelete(string files)
        {
            var folderPath = Path.Combine(_env.WebRootPath, imagefolder);
            var filename = Path.GetFileName(files);

            if (!string.IsNullOrEmpty(filename))
            {
                var fullName = Path.Combine(folderPath, filename);
                if (System.IO.File.Exists(fullName))
                {
                    System.IO.File.Delete(fullName);
                }
            }

            return Ok(new { deleted = "deleted" });
        }
    }
}