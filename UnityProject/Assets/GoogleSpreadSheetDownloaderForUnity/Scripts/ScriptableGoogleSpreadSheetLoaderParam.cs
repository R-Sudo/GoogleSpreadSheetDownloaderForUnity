using UnityEngine;

namespace GoogleSpreadSheetDownloaderForUnity
{
    [CreateAssetMenu(
    fileName = "GoogleSpreadSheetLoaderParam",
    menuName = "ScriptableObject/GoogleSpreadSheetLoaderParam",
    order = 0)]
    public class ScriptableGoogleSpreadSheetLoaderParam : ScriptableObject, IGoogleSpreadSheetLoaderParam
    {
        #region SpreadSheetId

        [SerializeField]
        private string _spreadSheetId;

        public string SpreadSheetId
        {
            get => _spreadSheetId;

#if UNITY_EDITOR
            set => _spreadSheetId = value;
#endif
        }

        #endregion

        #region SpreadSheetName

        [SerializeField]
        private string _spreadSheetName = string.Empty;

        public string SpreadSheetName
        {
            get => _spreadSheetName;

#if UNITY_EDITOR
            set => _spreadSheetId = value;
#endif
        }

        #endregion

        #region ApiKey

        [SerializeField]
        private string _apiKey;

        public string ApiKey
        {
            get => _apiKey;

#if UNITY_EDITOR
            set => _apiKey = value;
#endif
        }

        #endregion

        #region Range

        [SerializeField]
        private string _range;

        public string Range
        {
            get => _range;

#if UNITY_EDITOR
            set => _range = value;
#endif
        }

        #endregion
    }
}


