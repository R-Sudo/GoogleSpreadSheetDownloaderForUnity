using System.Collections.Generic;

namespace GoogleSpreadSheetDownloaderForUnity
{
    public interface ICsvData
    {
        IDictionary<string, IDictionary<string, string>> Data { get; }
    }

    internal class CsvData : ICsvData
    {
        public IDictionary<string, IDictionary<string, string>> Data { get; } = new Dictionary<string, IDictionary<string, string>>();
    }
}