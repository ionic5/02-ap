using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class RepeatTimer
    {
        private readonly Core.Timer _timer;

        private Action _task;
        private float _intervalSeconds;
        private int _remainingRepeats;

        public RepeatTimer(Core.Timer timer)
        {
            _timer = timer;
        }

        public void Start(Action task, float intervalSeconds, int repeatCount)
        {
            _task = task;
            _intervalSeconds = intervalSeconds;
            _remainingRepeats = repeatCount;

            ExecuteStep();
        }

        private void ExecuteStep()
        {
            if (_remainingRepeats <= 0)
            {
                Stop();
                return;
            }

            _task?.Invoke();
            _remainingRepeats--;

            if (_remainingRepeats > 0)
                _timer.Start(_intervalSeconds, ExecuteStep);
            else
                Stop();
        }

        public void Stop()
        {
            _task = null;
            _intervalSeconds = 0.0f;
            _remainingRepeats = 0;
            _timer.Stop();
        }
    }
}
