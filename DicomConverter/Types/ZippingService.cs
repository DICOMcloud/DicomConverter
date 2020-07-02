using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace DicomConverter.Types
{
    public static class ZippingService
    {
        public static byte[] CreateZipData(IEnumerable<ConvertedFileInfo> contents)
        {
            var ms = new MemoryStream();
            using (ms)
            {
                using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, false))
                {
                    foreach (var fileContent in contents)
                    {
                        var entry = zipArchive.CreateEntry(fileContent.FileName);

                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(fileContent.Data, 0, fileContent.Data.Length);
                            //using (var streamWriter = new StreamWriter(entryStream))
                            //{
                            //    streamWriter.Write(fileContent.Data);
                            //}
                        }
                    }
                }

                return ms.ToArray();
            }
        }
    }
}