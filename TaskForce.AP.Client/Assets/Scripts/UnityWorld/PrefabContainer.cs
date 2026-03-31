using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    [Serializable]
    public class PrefabContainer
    {
        [SerializeField]
        public string AssetID;
        [SerializeField]
        public GameObject Prefab;
    }
}
