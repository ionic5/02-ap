using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class SoulFinder : ISoulFinder
    {
        private readonly List<Soul> _souls;

        public SoulFinder()
        {
            _souls = new List<Soul>();
        }

        public int FindRadius(Vector2 center, float radius, List<Soul> results)
        {
            var detected = FindInRadius(center, radius, (entry) => true);
            results.AddRange(detected);
            return detected.Count();
        }

        public IEnumerable<Soul> FindInRadius(System.Numerics.Vector2 center, float radius, Func<Soul, bool> predicate)
        {
            float cx = center.X;
            float cy = center.Y;
            float r = radius;
            float rSqr = r * r;

            var detected = _souls.Where(predicate).Where(obj =>
            {
                var pos = obj.GetPosition();
                float dx = pos.X - cx;
                float dy = pos.Y - cy;

                if (dx < -r || dx > r || dy < -r || dy > r)
                    return false;

                return dx * dx + dy * dy <= rSqr;
            });

            return detected;
        }

        public void OnSoulCreatedEvent(object sender, CreatedEventArgs<Soul> args)
        {
            var soul = args.CreatedObject;

            _souls.Add(soul);
            soul.DestroyEvent += OnDestroySoulEvent;
        }

        private void OnDestroySoulEvent(object sender, DestroyEventArgs args)
        {
            var soul = _souls.FirstOrDefault(entry => entry == args.TargetObject);
            if (soul == null)
                return;

            soul.DestroyEvent -= OnDestroySoulEvent;
            _souls.Remove(soul);
        }

        public void Destroy()
        {
            foreach (var soul in _souls)
                soul.DestroyEvent -= OnDestroySoulEvent;

            _souls.Clear();
        }
    }
}
