using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TaskForce.AP.Client.UnityWorld
{
    public class AssetLoader
    {
        private readonly ConcurrentDictionary<string, AsyncOperationHandle> _loadedHandles;
        private readonly AssetPathStore _assetPathStore;
        private readonly Core.ILogger _logger;

        public AssetLoader(AssetPathStore assetPathStore, Core.ILogger logger)
        {
            _assetPathStore = assetPathStore;
            _logger = logger;
            _loadedHandles = new ConcurrentDictionary<string, AsyncOperationHandle>();
        }

        public async Task<T> LoadAsset<T>(string assetID) where T : UnityEngine.Object
        {
            return await LoadAssetByPath<T>(_assetPathStore.GetAssetPath(assetID));
        }

        public async Task<T> LoadAsset<T>(string assetID, System.Threading.CancellationToken token) where T : UnityEngine.Object
        {
            return await LoadAssetByPath<T>(_assetPathStore.GetAssetPath(assetID), token);
        }

        public async Task<Font> LoadFont(string languageCode)
        {
            return await LoadAssetByPath<Font>(_assetPathStore.GetAssetPathByLanguageCode(languageCode));
        }

        public async Task<T> LoadAssetByPath<T>(string path) where T : UnityEngine.Object
        {
            return await LoadAssetByPath<T>(path, System.Threading.CancellationToken.None);
        }

        public async Task<T> LoadAssetByPath<T>(string path, System.Threading.CancellationToken token) where T : UnityEngine.Object
        {
            AsyncOperationHandle handle = _loadedHandles.GetOrAdd(path, (key) =>
            {
                var h = Addressables.LoadAssetAsync<T>(key);
                h.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Failed)
                        _logger.Fatal($"Asset load failed: {path}. Error: {op.OperationException}");
                };
                return h;
            });

            try
            {
                if (!handle.IsDone)
                {
                    if (token.CanBeCanceled)
                    {
                        var completed = await Task.WhenAny(handle.Task, Task.Delay(-1, token));
                        if (completed != handle.Task)
                        {
                            TryRemoveHandle(path);
                            throw new System.OperationCanceledException(token);
                        }
                    }
                    else
                    {
                        await handle.Task;
                    }
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result as T;
                }
                else
                {
                    TryRemoveHandle(path);
                    _logger.Fatal($"Asset Load Failed ({typeof(T).Name}): {path}");
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                TryRemoveHandle(path);
                _logger.Fatal($"Error during Asset Load ({path}): {ex.Message}");
                return null;
            }
        }

        private void TryRemoveHandle(string path)
        {
            if (_loadedHandles.TryRemove(path, out AsyncOperationHandle handle))
                Addressables.Release(handle);
        }

        public void ClearAllAssets()
        {
            foreach (var pair in _loadedHandles)
                if (pair.Value.IsValid())
                    Addressables.Release(pair.Value);

            _loadedHandles.Clear();
            _logger.Info("All loaded asset data released.");
        }
    }
}
