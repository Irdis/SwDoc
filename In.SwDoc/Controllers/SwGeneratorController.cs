using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using In.SwDoc.Generator;
using In.SwDoc.Model;
using Microsoft.AspNetCore.Mvc;

namespace In.SwDoc.Controllers
{
    [Route("api/sw-generator")]
    public class SwGeneratorController : Controller
    {
        private readonly DocumentStorage _storage;

        public SwGeneratorController()
        {
            _storage = DocumentStorageFactory.Get();
        }

        [HttpPost("url")]
        public IActionResult GetDocument([FromBody]UrlForm data)
        {
            var generator = new DocGenerator();
            var request = WebRequest.Create(data.Url);
            request.Method = "GET";
            using (var responce = request.GetResponse())
            using (var stream = responce.GetResponseStream())
            using (var reader= new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                var d = generator.ConvertJsonToPdf(content);
                var id = _storage.SaveDocument(d);
                return Ok(new
                {
                    id
                });
            }
        }

        [HttpGet("document/{id}")]
        public IActionResult DownloadDocument(string id)
        {
            var stream = _storage.GetDocument(id);
            return File(stream, "application/pdf", "api-documentation.pdf");
        }
    }
}
