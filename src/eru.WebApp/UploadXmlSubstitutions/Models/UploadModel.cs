using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.UploadXmlSubstitutions.Models
{
    public class UploadModel
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
        [FromHeader(Name = "Api-Key")]
        public string ApiKey { get; set; }
    }
}