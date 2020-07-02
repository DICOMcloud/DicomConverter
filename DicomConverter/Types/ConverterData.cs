using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DicomConverter.Types
{
    public class ConverterData
    {
        public List<ConvertedFileInfo> Files { get; set; } = new List<ConvertedFileInfo> ();
        public string Format { get; set; }

        public Dictionary<string, string> Options { get; private set; } = new Dictionary<string, string> ( );
    }

    public class ConvertedFileInfo
    {
        public string FileName { get; set; }

        public byte[] Data { get; set; }
    }
}