using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class ObjectFactory : MonoBehaviour
    {
        public Core.ILogger Logger;

        [SerializeField]
        private GameObject _defaultRoot;
        [SerializeField]
        private PrefabContainer[] _prefabContainers;

        private Dictionary<string, Action<UnityWorld.Object>> _prepareHandlers;
        private Dictionary<string, Queue<PoolableObject>> _objectPools;

        [Serializable]
        public class PrefabContainer
        {
            [SerializeField]
            public string ObjectID;

            [SerializeField]
            public GameObject Prefab;

            [SerializeField]
            public GameObject Root;
        }

        private void Awake()
        {
            _prepareHandlers = new Dictionary<string, Action<UnityWorld.Object>>();
            _objectPools = new Dictionary<string, Queue<PoolableObject>>();
        }

        public void RegisterPrepareHandler(string objectID, Action<UnityWorld.Object> handler)
        {
            if (string.IsNullOrEmpty(objectID) || handler == null)
            {
                Logger.Fatal("Invalid prepare handler registration.");
                return;
            }
            _prepareHandlers[objectID] = handler;
        }

        public T Create<T>(string objectID) where T : UnityWorld.Object
        {
            if (string.IsNullOrEmpty(objectID))
            {
                Logger.Fatal("Object ID is null or empty.");
                return null;
            }

            var newObj = GetNewObject<T>(objectID) ?? CreateNewObject<T>(objectID);

            if (newObj != null && _prepareHandlers.TryGetValue(objectID, out var handler))
                handler?.Invoke(newObj);

            return newObj;
        }

        private T CreateNewObject<T>(string objectID) where T : UnityWorld.Object
        {
            var container = _prefabContainers.FirstOrDefault(c => c.ObjectID == objectID);
            if (container == null || container.Prefab == null)
            {
                Logger.Fatal($"Object Creation Failed: ID '{objectID}' not found in Pool or Prefab list.");
                return null;
            }

            var root = container.Root == null ? _defaultRoot.transform : container.Root.transform;
            GameObject go = Instantiate(container.Prefab, root);
            if (!go.TryGetComponent<T>(out T newObj))
            {
                Logger.Fatal($"Component {typeof(T)} not found on instantiated prefab '{objectID}'.");
                return null;
            }

            if (newObj is PoolableObject poolable)
                poolable.SetReturnToPoolHandler(_ => Release(objectID, poolable));

            return newObj;
        }

        private T GetNewObject<T>(string objectID) where T : UnityWorld.Object
        {
            if (!_objectPools.TryGetValue(objectID, out var pool) || pool.Count <= 0)
                return null;

            var pooledObj = pool.Dequeue();
            if (pooledObj == null)
                return null;

            pooledObj.gameObject.SetActive(true);
            pooledObj.Revive();

            pooledObj.SetReturnToPoolHandler(_ => Release(objectID, pooledObj));

            return pooledObj as T;
        }

        private void Release(string objectID, UnityWorld.PoolableObject obj)
        {
            if (string.IsNullOrEmpty(objectID) || obj == null)
                return;

            obj.gameObject.SetActive(false);

            if (!_objectPools.ContainsKey(objectID))
                _objectPools[objectID] = new Queue<UnityWorld.PoolableObject>();

            _objectPools[objectID].Enqueue(obj);
        }
    }
}
