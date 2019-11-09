using System;
using UniRx.Async;

namespace GoogleSpreadSheetDownloaderForUnity
{
    interface IAsyncCsvLoader
    {
        UniTask<ICsvData> LoadAsync();
        UniTask<string> LoadAsyncAsRawString();

        event Action PostLoad;
    }
}
