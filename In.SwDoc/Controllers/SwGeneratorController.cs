using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using In.SwDoc.Generator;
using Microsoft.AspNetCore.Mvc;

namespace In.SwDoc.Controllers
{
    [Route("api/sw-generator")]
    public class SwGeneratorController : Controller
    {
        [HttpPost("text")]
        public IActionResult GetDocument(string data)
        {
            var generator = new DocGenerator();
            var d = generator.ConvertJsonToPdf(data);
            return File(d, "application/pdf", "api-documentation.pdf");
        }
    }
}
