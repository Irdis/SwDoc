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
        private readonly DocGenerator _generator;

        public SwGeneratorController()
        {
            _storage = DocumentStorageFactory.Get();
            _generator = DocGeneratorFactory.Get();
        }

        [HttpPost("url")]
        public IActionResult GetDocumentByUrl([FromBody]UrlForm data)
        {
            var request = WebRequest.Create(data.Url);
            request.Method = "GET";
            using (var responce = request.GetResponse())
            using (var stream = responce.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                var d = _generator.ConvertJsonToPdf(content);
                var id = _storage.SaveDocument(d);
                return Ok(new
                {
                    id
                });
            }
        }

        [HttpPost("spec")]
        public IActionResult GetDocumentBySpec([FromBody]SpecForm data)
        {
            var d = _generator.ConvertJsonToPdf(data.Text);
            var id = _storage.SaveDocument(d);
            return Ok(new
            {
                id
            });

        }

        [HttpGet("document/{id}")]
        public IActionResult DownloadDocument(string id)
        {
            var stream = _storage.GetDocument(id);
            return File(stream, "application/pdf", "api-documentation.pdf");
        }
    }
}
