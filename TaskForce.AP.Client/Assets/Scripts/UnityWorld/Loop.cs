using System.Collections.Generic;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class Loop : MonoBehaviour, ILoop
    {
        public readonly LinkedList<IUpdatable> updatables = new LinkedList<IUpdatable>();

        public void Add(IUpdatable updatable)
        {
            updatables.AddLast(updatable);
        }

        public void Remove(IUpdatable updatable)
        {
            updatables.Remove(updatable);
        }

        private void OnDestroy()
        {
            updatables.Clear();
        }

        private void Update()
        {
            var node = updatables.First;
            while (node != null)
            {
                var current = node;
                node = node.Next;
                current.Value.Update();
            }
        }
    }
}
