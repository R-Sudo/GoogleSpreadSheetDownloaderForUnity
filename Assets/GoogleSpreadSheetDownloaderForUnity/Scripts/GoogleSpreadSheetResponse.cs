using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GoogleSpreadSheetDownloaderForUnity
{
    [Serializable]
    public class GoogleSpreadSheetResponse
    {
        [DataMember(Name = "range")]
        public string Range { get; set; }
        
        [DataMember(Name = "majorDimension")]
        public string MajorDimension { get; set; }

        [DataMember(Name = "values")]
        public List<List<string>> Values { get; set; }
    }
}