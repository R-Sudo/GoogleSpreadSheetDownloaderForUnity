using System;
using System.Collections.Generic;
using System.Net.Http;
using UniRx.Async;
using UnityEngine;
using Utf8Json;

namespace GoogleSpreadSheetDownloaderForUnity
{
    public class GoogleSpreadSheetLoaderForHttp : IAsyncCsvLoader
    {
        private readonly IGoogleSpreadSheetLoaderParam _param;

        public event Action PostLoad;

        public GoogleSpreadSheetLoaderForHttp(IGoogleSpreadSheetLoaderParam param)
        {
            _param = param;
        }

        public async UniTask<ICsvData> LoadAsync()
        {
            if(ParamValidate() is false)
                return null;

            var spreadSheetJson = await DownloadSpreadSheet();

            if(spreadSheetJson is null)
                return null;

            var response = ParseSpreadSheetJson(spreadSheetJson);

            if (response is null)
                return null;

            var result =  ParseResponseAsCsvData(response);
            PostLoad?.Invoke();
            return result;
        }

        public async UniTask<string> LoadAsyncAsRawString()
        {
            if(ParamValidate() is false)
                return null;

            var spreadSheetJson = await DownloadSpreadSheet();

            if(spreadSheetJson is null)
                return null;

            var response = ParseSpreadSheetJson(spreadSheetJson);

            if(response is null)
                return null;

            var result =  ParseResponseAsRawCsvString(response);
            PostLoad?.Invoke();
            return result;
        }

        /// <summary>
        /// GoogleSpreadSheet をHttp リクエストで取得する
        /// </summary>
        /// <returns>
        /// レスポンスのJson 文字列
        /// </returns>
        private async UniTask<string> DownloadSpreadSheet()
        {
            const string requestUriBase = @"https://sheets.googleapis.com/v4/spreadsheets/";
            string spreadSheetString;

            using (var client = new HttpClient())
            {
                var range = _param.Range ?? string.Empty;
                var requestUri = $@"{requestUriBase}{_param.SpreadSheetId}/values/{_param.SpreadSheetName}{range}?key={_param.ApiKey}";

                try
                {
                    var response = await client.GetAsync(requestUri);
                    spreadSheetString = await response.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return null;
                }
            }

            return spreadSheetString;
        }

        private GoogleSpreadSheetResponse ParseSpreadSheetJson(string jsonString)
        {
            // TODO: async 版のデシリアライズを使う？
            var deserializeObj = JsonSerializer.Deserialize<GoogleSpreadSheetResponse>(jsonString);

            if (deserializeObj?.Values is null)
            {
                Debug.LogError("Json のデシリアライズに失敗");
                return null;
            }

            return deserializeObj;
        }

        private ICsvData ParseResponseAsCsvData(GoogleSpreadSheetResponse response)
        {
            var csvData = new CsvData();

            // NOTE: 先頭行・列はKey として取り扱う
            try
            {
                var header = response.Values[0];
                header.RemoveAt(0);
                response.Values.RemoveAt(0);

                foreach(var row in response.Values)
                {
                    var rowKey = row[0];
                    row.RemoveAt(0);

                    var rowData = new Dictionary<string, string>();
                    csvData.Data.Add(rowKey, rowData);

                    // データが不足しているので足りない分は空データをいれておく
                    if (row.Count < header.Count)
                    {
                        while(row.Count != header.Count)
                            row.Add(string.Empty);
                    }

                    for(var i = 0 ; i < header.Count ; i++)
                    {
                        var value = row[i];

                        rowData.Add(header[i], value);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e}:{e.Message}");
            }

            return csvData;
        }

        private string ParseResponseAsRawCsvString(GoogleSpreadSheetResponse response)
        {
            string rawCsvString = null;

            try
            {
                var header = response.Values[0];

                foreach(var row in response.Values)
                {
                    var validateRow = new List<string>();

                    // データが不足しているので足りない分は空データをいれておく
                    if(row.Count < header.Count)
                    {
                        while(row.Count != header.Count)
                            row.Add(string.Empty);
                    }

                    foreach(var column in row)
                    {
                        var validateColumn = column;

                        // csv で複数行を扱うセルに関して、""で囲われた範囲が1セルになる。
                        // 更に、" が文字列として含まれている場合は"" とすることでエスケープされる。
                        if(column.Contains("\n"))
                        {
                            validateColumn = column.Replace("\"", "\"\"");
                            validateColumn = $"\"{validateColumn}\"";
                        }

                        validateRow.Add(validateColumn);
                    }

                    var rowString = string.Join(",", validateRow) + "\n";
                    rawCsvString += rowString;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e}:{e.Message}");
            }

            return rawCsvString;
        }

        private bool ParamValidate()
        {
            if (_param is null)
            {
                Console.WriteLine("パラメーターが未設定です。");
                return false;
            }

            if (string.IsNullOrEmpty(_param.SpreadSheetName))

            {
                Console.WriteLine("スプレッドシート名が未設定です。");
                return false;
            }

            if(string.IsNullOrEmpty(_param.SpreadSheetId))
            {
                Console.WriteLine("スプレッドシートID が未設定です。");
                return false;
            }

            if(string.IsNullOrEmpty(_param.ApiKey))
            {
                Console.WriteLine("API キーが未設定です。");
                return false;
            }

            return true;
        }
    }
}