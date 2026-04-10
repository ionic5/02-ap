using UnityEngine;
using UnityEngine.Events;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class UnitAnimationEventListener : MonoBehaviour
    {
        public UnityEvent UnitDiedAnimationFinished;

        public void OnDieAnimationFinished()
        {
            UnitDiedAnimationFinished?.Invoke();
        }
    }
}