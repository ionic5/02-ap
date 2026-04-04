using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class FieldObjectFinder : IFieldObjectFinder
    {
        private readonly List<IFieldObject> _fieldObjects = new List<IFieldObject>();

        public int FindRadius(Vector2 center, float radius, List<IFieldObject> results)
        {
            float cx = center.X;
            float cy = center.Y;
            float rSqr = radius * radius;

            var detected = _fieldObjects.Where(obj =>
            {
                var pos = obj.GetPosition();
                float dx = pos.X - cx;
                float dy = pos.Y - cy;

                if (dx < -radius || dx > radius || dy < -radius || dy > radius)
                    return false;

                return dx * dx + dy * dy <= rSqr;
            }).ToList();

            results.AddRange(detected);
            return detected.Count;
        }

        public void OnExpOrbCreatedEvent(object sender, CreatedEventArgs<ExpOrb> args)
        {
            Register(args.CreatedObject);
        }

        private void Register(IFieldObject fieldObject)
        {
            _fieldObjects.Add(fieldObject);
            fieldObject.DestroyEvent += OnDestroyEvent;
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs args)
        {
            var obj = _fieldObjects.FirstOrDefault(entry => entry == args.TargetObject);
            if (obj == null) return;

            obj.DestroyEvent -= OnDestroyEvent;
            _fieldObjects.Remove(obj);
        }

        public void Destroy()
        {
            foreach (var obj in _fieldObjects)
                obj.DestroyEvent -= OnDestroyEvent;
            _fieldObjects.Clear();
        }
    }
}
