namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface ITarget : IDestroyable, IMortal, IPositionable, IFollowable
    {
        void Hit(IUnit attacker, int damage);
        void Kill(IUnit killer);
        bool IsAlive();
        bool IsPlayerSide();
        string GetViewID();
        int GetRemainHP();
        bool IsFullHP();
        void Heal(int healAmount);
    }
}
