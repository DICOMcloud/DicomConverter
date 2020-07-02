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
            List<ConvertedFileInfo> convertedDcm = new List<ConvertedFileInfo>();
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
                    foreach (var file in data.Files)
                    {
                        var converter = new JsonDicomConverter();
                        var dcmFile = Dicom.DicomFile.Open(new MemoryStream(file.Data));

                        converter.WriteInlineBinary = true;

                        convertedDcm.Add(new ConvertedFileInfo( ) 
                        { 
                            FileName = Path.ChangeExtension(file.FileName, KnownFormats.JSON),
                            Data = Encoding.UTF8.GetBytes(converter.Convert(dcmFile.Dataset))
                        });
                    }
                }
                break;

                case KnownFormats.XML:
                {
                    foreach (var file in data.Files)
                    {
                        var converter = new XmlDicomConverter();
                        var dcmFile = Dicom.DicomFile.Open(new MemoryStream(file.Data));

                        converter.WriteInlineBinary = true;

                        convertedDcm.Add(new ConvertedFileInfo( ) 
                        { 
                            FileName = Path.ChangeExtension(file.FileName, KnownFormats.XML),
                            Data = Encoding.UTF8.GetBytes(converter.Convert(dcmFile.Dataset))
                        });
                    }
                }
                break;

                default:
                {
                    responseContent.Status = System.Net.HttpStatusCode.UnsupportedMediaType;
                }
                break;
            }

            responseContent.FileName = Path.ChangeExtension(convertedDcm.First().FileName, "zip");
            responseContent.Content = new ByteArrayContent(ZippingService.CreateZipData(convertedDcm));
            responseContent.Status = System.Net.HttpStatusCode.OK;
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