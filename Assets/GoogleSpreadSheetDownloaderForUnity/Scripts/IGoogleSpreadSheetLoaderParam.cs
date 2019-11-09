namespace GoogleSpreadSheetDownloaderForUnity
{
    public interface IGoogleSpreadSheetLoaderParam
    {
        string SpreadSheetId { get; }
        string SpreadSheetName { get; }
        string ApiKey { get; }
        string Range { get; }
    }
}