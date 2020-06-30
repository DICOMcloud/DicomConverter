using DICOMcloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace DicomConverter.Types
{
    public class DicomConverterService
    {
        public ResponseContent Convert ( ConverterData data)
        {
            List<string> convertedDcm = new List<string>();
            ResponseContent responseContent = new ResponseContent ( );

            if (data.Files.Count == 0)
            {
                responseContent.Status = System.Net.HttpStatusCode.NoContent;

                return responseContent;
            }

            switch (data.Format)
            {
                case KnownFormats.JSON:
                {
                    string fileName = "";

                    foreach (var file in data.Files)
                    {
                        var converter = new JsonDicomConverter();
                        var dcmFile = Dicom.DicomFile.Open(new MemoryStream(file.Data));

                        converter.WriteInlineBinary = true;

                        convertedDcm.Add(converter.Convert(dcmFile.Dataset));

                        fileName = file.FileName;
                    }

                    responseContent.FileName = Path.ChangeExtension(fileName, KnownFormats.JSON);
                    responseContent.Content = new StringContent("[" + string.Join(",", convertedDcm) + "]", Encoding.UTF8, "application/json");

                    responseContent.Status = System.Net.HttpStatusCode.OK;
                }
                break;

                default:
                {
                    responseContent.Status = System.Net.HttpStatusCode.UnsupportedMediaType;
                }
                break;
            }

            return responseContent;
        }
    }

    static class KnownFormats
    {
        public const string JSON = "json";
        public const string XML = "xml";
        public const string JPG = "jpg";
    }
}