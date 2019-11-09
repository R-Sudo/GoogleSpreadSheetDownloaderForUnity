using System;
using System.IO;
using UnityEngine;

namespace GoogleSpreadSheetDownloaderForUnity
{
    public static class CsvUtil
    {
        public static void SaveTo(string saveFilePath, string rawCsvString)
        {
            try
            {
                var directoryName = Path.GetDirectoryName(saveFilePath);

                if(Directory.Exists(directoryName) is false)
                {
                    Debug.LogError($"Save Failed\nPath:{saveFilePath}");
                    return;
                }

                File.WriteAllText(saveFilePath, rawCsvString);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save Failed\nMessage: {e.Message}");
                return;
            }
        }
    }
}