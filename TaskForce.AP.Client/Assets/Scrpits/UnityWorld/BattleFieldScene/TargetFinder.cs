using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class TargetFinder : ITargetFinder
    {
        private readonly List<ITarget> _targets;

        public TargetFinder()
        {
            _targets = new List<ITarget>();
        }

        public void OnTargetCreatedEvent(object sender, CreatedEventArgs<Unit> args)
        {
            var target = args.CreatedObject;

            _targets.Add(target);
            target.DiedEvent += OnTargetDiedEvent;
            target.DestroyEvent += OnDestroyTargetEvent;
        }

        public IEnumerable<ITarget> FindInRadius(Vector2 center, float minRadius, float maxRadius, Func<ITarget, bool> predicate)
        {
            float cx = center.X;
            float cy = center.Y;
            float minSqr = minRadius * minRadius;
            float maxSqr = maxRadius * maxRadius;

            var detected = _targets.Where(predicate).Where(obj =>
            {
                var pos = obj.GetPosition();
                float dx = pos.X - cx;
                float dy = pos.Y - cy;

                if (dx < -maxRadius || dx > maxRadius || dy < -maxRadius || dy > maxRadius)
                    return false;

                float distSqr = dx * dx + dy * dy;

                return distSqr >= minSqr && distSqr <= maxSqr;
            });

            return detected;

        }

        public IEnumerable<ITarget> FindInRadius(System.Numerics.Vector2 center, float radius, Func<ITarget, bool> predicate)
        {
            return FindInRadius(center, 0.0f, radius, predicate);
        }

        public IEnumerable<ITarget> FindInSector(System.Numerics.Vector2 center, System.Numerics.Vector2 direction, float degree, float radius, Func<ITarget, bool> predicate)
        {
            var inRadius = FindInRadius(center, radius, predicate);

            var dirNorm = System.Numerics.Vector2.Normalize(direction);
            var cosThreshold = Math.Cos(degree * 0.5f * (Math.PI / 180.0));

            return inRadius.Where(obj =>
            {
                var toTargetNorm = System.Numerics.Vector2.Normalize(obj.GetPosition() - center);
                float cosValue = System.Numerics.Vector2.Dot(dirNorm, toTargetNorm);
                return cosValue >= cosThreshold;
            });
        }

        public void Destroy()
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
                RemoveTarget(_targets[i]);

            _targets.Clear();
        }

        public ITarget FindByViewID(string objectID)
        {
            return _targets.FirstOrDefault(entry => entry.GetViewID() == objectID);
        }


        public IEnumerable<ITarget> FindByViewIDs(IEnumerable<string> objectIDs, Func<ITarget, bool> predicate)
        {
            var ids = new HashSet<string>(objectIDs);
            return _targets.Where(entry => ids.Contains(entry.GetViewID())).Where(predicate);
        }

        public void OnDestroyTargetEvent(object sender, DestroyEventArgs args)
        {
            var target = _targets.FirstOrDefault(entry => entry == args.TargetObject);
            if (target != null)
                RemoveTarget(target);
        }

        public void OnTargetDiedEvent(object sender, DiedEventArgs args)
        {
            var target = _targets.FirstOrDefault(entry => entry == args.DiedTarget);
            if (target == null)
                return;

            RemoveTarget(target);
        }

        private void RemoveTarget(ITarget target)
        {
            if (!_targets.Contains(target))
                return;

            target.DestroyEvent -= OnDestroyTargetEvent;
            target.DiedEvent -= OnTargetDiedEvent;

            _targets.Remove(target);
        }
    }
}
