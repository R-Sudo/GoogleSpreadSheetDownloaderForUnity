using System.IO;
using GoogleSpreadSheetDownloaderForUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.GoogleSpreadSheetDownloaderForUnity.Editor.Common.GoogleSpreadSheet
{
    public class GoogleSpreadSheetDownloadWindow : EditorWindow
    {
        private ScriptableGoogleSpreadSheetLoaderParam _downloadParam;
        private UnityEngine.Object _saveToFolder;
        private string _saveFileName;
        private bool _isDownloading;
        private bool _isEnable;
        private GameObject _downLoaderObject;
        private string _saveFilePath;

        [MenuItem("Window/GoogleSpreadSheetDownloaderForUnity")]
        static void Init()
        {
            var win = GetWindow<GoogleSpreadSheetDownloadWindow>();
            win.minSize = win.maxSize = new Vector2(400, 120);

            win._isEnable = EditorApplication.isPlaying;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnPlayModeChanged(PlayModeStateChange args)
        {
            if (args == PlayModeStateChange.EnteredPlayMode)
            {
                _isEnable = true;
            }

            if (args == PlayModeStateChange.ExitingPlayMode)
            {
                _isEnable = false;
            }

        }

        private void OnGUI()
        {
            GUI.enabled = _isDownloading is false && _isEnable is true;
            EditorGUIUtility.labelWidth = 80;

            // ダウンロード引数設定
            EditorGUI.BeginChangeCheck();

            var loadParam = EditorGUILayout.ObjectField("LoadParam", _downloadParam, typeof(ScriptableGoogleSpreadSheetLoaderParam), false);

            if (EditorGUI.EndChangeCheck() is true)
            {
                if (loadParam is ScriptableGoogleSpreadSheetLoaderParam scriptableLoadParam)
                {
                    _downloadParam = scriptableLoadParam;
                }
            }

            // 保存先フォルダ
            EditorGUI.BeginChangeCheck();

            var saveToFolder = EditorGUILayout.ObjectField("SaveFolder", _saveToFolder, typeof(UnityEngine.Object), false);

            if(EditorGUI.EndChangeCheck() is true)
            {
                if(saveToFolder != null)
                {
                    var path = AssetDatabase.GetAssetPath(saveToFolder);
                    var attr = File.GetAttributes(path);

                    if((attr & FileAttributes.Directory) != 0)
                    {
                        _saveToFolder = saveToFolder;
                    }
                }
                else
                {
                    _saveToFolder = null;
                }
            }

            // 保存ファイル名
            _saveFileName = EditorGUILayout.TextField("FileName", _saveFileName);

            // ダウンロードボタン
            if(GUILayout.Button("Download"))
            {
                Download();
            }

            GUI.enabled = true;
        }

        private void Download()
        {
            var saveDirectoryPath = AssetDatabase.GetAssetPath(_saveToFolder)?.Replace('/', Path.DirectorySeparatorChar);

            if (string.IsNullOrEmpty(saveDirectoryPath))
            {
                Debug.LogError("Failed GoogleSpreadSheet Download Start");
                return;
            }

            _saveFilePath = Path.Combine(saveDirectoryPath, _saveFileName) + ".csv";

            _downLoaderObject = new GameObject("DownLoader");
            _downLoaderObject.AddComponent<GoogleSpreadSheetDownLoader>();

            var googleSpreadSheetDownLoader = _downLoaderObject.GetComponent<GoogleSpreadSheetDownLoader>();
            googleSpreadSheetDownLoader.LoaderParam = _downloadParam;
            googleSpreadSheetDownLoader.SaveFilePath = _saveFilePath;
            googleSpreadSheetDownLoader.Completed += OnComplete;

            Debug.Log("GoogleSpreadSheet Download Start");
            googleSpreadSheetDownLoader.LoadStart();

            _isDownloading = true;
        }

        private void OnComplete()
        {
            _isDownloading = false;
            Destroy(_downLoaderObject.gameObject);
            Debug.Log($"GoogleSpreadSheet Download Complete: {_saveFilePath}");
        }
    }
}
