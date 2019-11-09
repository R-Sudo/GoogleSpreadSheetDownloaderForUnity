using System.Collections;
using UniRx.Async;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace GoogleSpreadSheetDownloaderForUnity.Tests
{
    public class GoogleSpreadSheetLoaderTest
    {
        private class TestGoogleSpreadSheetLoaderParam : IGoogleSpreadSheetLoaderParam
        {
            public string SpreadSheetId { get; set; }
            public string SpreadSheetName { get; set; }
            public string ApiKey { get; set; }
            public string Range { get; set; } = string.Empty;
        }

        /// <summary>
        /// 非同期ロードテスト
        /// </summary>
        [UnityTest]
        public IEnumerator AsyncLoadTest() => UniTask.ToCoroutine(async () =>
        {
            var loaderParam = SetupLoaderParam();

            var loader = new GoogleSpreadSheetLoaderForHttp(loaderParam);
            var result = await loader.LoadAsync();

            Assert.AreNotEqual(result, null);
            OutputDownloadData(result);
        });

        /// <summary>
        /// 非同期ロードテスト(生文字列版)
        /// </summary>
        [UnityTest]
        public IEnumerator AsyncLoadAsRawStringTest() => UniTask.ToCoroutine(async () =>
        {
            var loaderParam = SetupLoaderParam();

            var loader = new GoogleSpreadSheetLoaderForHttp(loaderParam);
            var result = await loader.LoadAsyncAsRawString();

            Assert.AreNotEqual(result, null);

            Debug.Log($"LoadSpreadSheetString\n{result}");
        });

        private IGoogleSpreadSheetLoaderParam SetupLoaderParam()
        {
            // ここに自身のアカウントのスプレッドシートのパラメーターを入れる
            return new TestGoogleSpreadSheetLoaderParam
            {
                SpreadSheetName = "",
                SpreadSheetId = "",
                ApiKey = ""
            };
        }

        private void OutputDownloadData(ICsvData downloadData)
        {
            // 表示してみる
            string loggerString = string.Empty;

            foreach(var row in downloadData.Data)
            {
                loggerString += "===================\n";
                loggerString += $"Key: {row.Key}\n";

                foreach(var column in row.Value)
                {
                    loggerString += $"{column.Key}: {column.Value}\n";
                }

                loggerString += "===================\n";
            }

            Debug.Log(loggerString);
        }
    }
}