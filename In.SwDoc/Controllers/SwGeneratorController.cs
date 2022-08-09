using System;
using System.IO;
using System.Net;
using In.SwDoc.Generator;
using In.SwDoc.Model;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace In.SwDoc.Controllers
{
    [Route("api/sw-generator")]
    public class SwGeneratorController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DocGenerator));
        private readonly IDocumentStorage _storage;
        private readonly IDocGenerator _generator;

        public SwGeneratorController(IDocumentStorage storage, IDocGenerator generator)
        {
            _storage = storage;
            _generator = generator;
        }

        [HttpPost("url")]
        public IActionResult GetDocumentByUrl([FromBody]UrlForm data)
        {
            try
            {
                var request = WebRequest.Create(data.Url);
                request.Method = "GET";
                using (var responce = request.GetResponse())
                using (var stream = responce.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    var d = _generator.ConvertJsonToPdf(content, data.OpenApi, data.Font);
                    var id = _storage.SaveDocument(d);
                    return Ok(new
                    {
                        id,
                        error = (string) null
                    });
                }
            }
            catch (WebException e)
            {
                return Ok(new
                {
                    id = (string) null,
                    error = "WebException"
                });
            }
            catch (DocumentGenerationException e)
            {
                return Ok(new
                {
                    id = (string) null,
                    error = "GenerationError"
                });
            }
            catch (Exception e)
            {
                _log.Error("Unknown exception", e);
                return Ok(new
                {
                    id = (string) null,
                    error = "UnknownException"
                });
            }
        }

        [HttpPost("spec")]
        public IActionResult GetDocumentBySpec([FromBody]SpecForm data)
        {
            try
            {
                var d = _generator.ConvertJsonToPdf(data.Text, data.OpenApi, data.Font);
                var id = _storage.SaveDocument(d);
                return Ok(new
                {
                    id,
                    error = (string) null
                });
            }
            catch (DocumentGenerationException e)
            {
                return Ok(new
                {
                    id = (string) null,
                    error = "GenerationError"
                });
            }
            catch (Exception e)
            {
                _log.Error("Unknown exception", e);
                return Ok(new
                {
                    id = (string) null,
                    error = "UnknownException"
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
