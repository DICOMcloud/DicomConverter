using DicomConverter.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DicomConverter.Controllers
{
    public class DicomController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/values/
        public async Task<HttpResponseMessage> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent())
            { 
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            
            var provider = new MultipartMemoryStreamProvider();

            await Request.Content.ReadAsMultipartAsync(provider);
            
            ConverterData converterData = new ConverterData ( );
            
            foreach (var file in provider.Contents)
            {
                if (file.Headers.ContentDisposition.FileName != null)
                { 
                    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();

                    if (buffer.Length == 0)
                    { 
                        continue;
                    }

                    converterData.Files.Add ( new DicomFileInfo (){  Data = buffer, FileName = filename});
                }
                else 
                { 
                    var disposition = file.Headers.ContentDisposition;

                    var value = await file.ReadAsStringAsync();

                    if (disposition.Name.Trim('\"') == "format")
                    {
                        converterData.Format = value;
                    }
                    else
                    {
                        converterData.Options.Add (disposition.Name, value);
                    }

                    System.Diagnostics.Trace.WriteLine(value);
                }
            }

            var responseContent = new DicomConverterService().Convert(converterData);

            var response = Request.CreateResponse(responseContent.Status);

            if (responseContent.Content != null)
            {
                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = responseContent.FileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };

                response.Content = responseContent.Content;
                response.Content.Headers.Add("Content-Disposition", cd.ToString());
            }

            return response;
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
