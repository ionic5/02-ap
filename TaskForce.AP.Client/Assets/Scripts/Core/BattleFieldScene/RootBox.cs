using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class RootBox : ITarget, IFieldObject
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;
        public event EventHandler<DiedEventArgs> DiedEvent;

        private bool _isDestroyed;
        private bool _isDead;
        private int _hp;
        private readonly View.BattleFieldScene.IRootBox _view;

        public RootBox(View.BattleFieldScene.IRootBox view, int hp)
        {
            _view = view;
            _hp = hp;
            _view.DestroyEvent += OnViewDestroyEvent;
        }

        public void Handle(IFieldObjectHandler handler)
        {
            handler.Handle(this);
        }

        public void Kill(IUnit killer)
        {
            if (_isDead)
                return;

            _hp = 0;
            _isDead = true;
            DiedEvent?.Invoke(this, new DiedEventArgs(this, killer));
            Destroy();
        }

        public void Hit(IUnit attacker, int damage)
        {
            if (_isDead)
                return;

            _hp -= damage;
            if (_hp > 0)
                return;

            _hp = 0;
            _isDead = true;
            DiedEvent?.Invoke(this, new DiedEventArgs(this));
            Destroy();
        }

        public bool IsAlive() => !_isDead;
        public bool IsDead() => _isDead;
        public bool IsPlayerSide() => false;
        public bool IsFullHP() => false;
        public int GetRemainHP() => _hp;
        public void Heal(int healAmount) { }

        public Vector2 GetPosition() => _view.GetPosition();
        public void SetPosition(Vector2 position) => _view.SetPosition(position);
        public string GetViewID() => _view.GetObjectID();

        private void OnViewDestroyEvent(object sender, DestroyEventArgs e) => Destroy();

        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            _view.DestroyEvent -= OnViewDestroyEvent;
            _view.Destroy();
        }
    }
}
