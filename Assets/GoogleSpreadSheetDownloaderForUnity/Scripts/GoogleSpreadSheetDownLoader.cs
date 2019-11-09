using System;
using UniRx.Async;
using UnityEngine;

namespace GoogleSpreadSheetDownloaderForUnity
{
    public class GoogleSpreadSheetDownLoader : MonoBehaviour
    {
        #region LoaderParam

        [SerializeField]
        private ScriptableGoogleSpreadSheetLoaderParam _loaderParam;

        public ScriptableGoogleSpreadSheetLoaderParam LoaderParam
        {
            get => _loaderParam;
            set => _loaderParam = value;
        }

        #endregion

        #region SaveFilePath

        [SerializeField]
        private string _saveFilePath;

        public string SaveFilePath
        {
            get => _saveFilePath;
            set => _saveFilePath = value;
        }

        #endregion

        public event Action Completed;

        public void LoadStart()
        {
            var loader = new GoogleSpreadSheetLoaderForHttp(LoaderParam);

            UniTask.Run(async () =>
            {
                var downloadDataRawString = await loader.LoadAsyncAsRawString();
                CsvUtil.SaveTo(SaveFilePath, downloadDataRawString);

                await UniTask.Yield();
                Completed?.Invoke();
            }).Forget();
        }
    }
}

