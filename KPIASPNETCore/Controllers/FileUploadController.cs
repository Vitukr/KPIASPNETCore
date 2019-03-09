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

        public FileUploadController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost("Uploadfile")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var folderPath = Path.Combine(_env.WebRootPath, "images"); //Path.GetTempFileName();
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
            var folderPath = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string[] fileEntries = Directory.GetFiles(folderPath);
            return fileEntries;
        }
    }
}