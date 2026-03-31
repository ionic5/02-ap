using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace TaskForce.AP.Client.UnityWorld
{
    public class Screen : MonoBehaviour
    {
        [SerializeField]
        private Canvas _loadingBlind;

        public Core.ILogger Logger;
        public AssetLoader AssetLoader;

        private TaskCompletionSource<bool> _loadingTcs = null;
        private volatile bool _isShowing = false;

        public async Task ShowLoadingBlind()
        {
            if (_isShowing)
            {
                if (_loadingTcs != null)
                {
                    await _loadingTcs.Task;
                    return;
                }
            }

            _loadingTcs = new TaskCompletionSource<bool>();
            _isShowing = true;
            _loadingBlind.gameObject.SetActive(true);
        }

        public void HideLoadingBlind()
        {
            if (!_isShowing) return;

            _loadingBlind.gameObject.SetActive(false);
            _isShowing = false;

            var tcs = _loadingTcs;
            _loadingTcs = null;
            tcs?.TrySetResult(true);
        }

        public async Task DestroyLastScene()
        {
            // Unity는 활성화된 씬이 없는 상태를 허용하지 않으므로, 
            // 기존 씬을 완전히 언로드하기 위해 '빈 씬(EmptyScene)'을 로드하여 대체합니다.
            if (!await TryLoadSceneAsync(SceneID.EmptyScene))
                return;

            await ClearAllResources();
        }

        private async Task ClearAllResources()
        {
            AssetLoader.ClearAllAssets();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            var unloadOp = Resources.UnloadUnusedAssets();
            while (!unloadOp.isDone)
                await Task.Yield();
        }

        private async Task<bool> TryLoadSceneAsync(string sceneID)
        {
            var op = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Single);
            if (op == null)
            {
                Logger.Fatal($"Failed to find scene. Scene id : {sceneID}");
                return false;
            }

            while (!op.isDone)
                await Task.Yield();

            return true;
        }

        public async Task<GameObject> AttachNewScene(string sceneID)
        {
            if (!await TryLoadSceneAsync(sceneID))
                return null;

            var loadedScene = SceneManager.GetSceneByName(sceneID);
            if (!loadedScene.IsValid())
                return null;

            SceneManager.SetActiveScene(loadedScene);

            var sceneObj = FindSceneObject(loadedScene);
            if (sceneObj == null)
                Logger.Fatal($"Failed to find scene object in scene. Scene id : {sceneID}");

            return sceneObj;
        }

        private static GameObject FindSceneObject(UnityEngine.SceneManagement.Scene loadedScene)
        {
            foreach (var rootObj in loadedScene.GetRootGameObjects())
                if (rootObj.TryGetComponent<Scene>(out _))
                    return rootObj;
            return null;
        }
    }
}
