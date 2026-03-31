using System;

namespace TaskForce.AP.Client.Core
{
    public class Timer : IUpdatable, IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        private readonly ITime _time;
        private readonly ILoop _loop;

        private bool _isRunning;
        private float _remainTime;
        private Action _timeOutHandler;
        private bool _isDestroyed;

        public Timer(ITime time, ILoop loop)
        {
            _time = time;
            _loop = loop;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public void Start(float time, Action timeOutHandler = null)
        {
            if (_isDestroyed) return;

            if (!_isRunning)
                _loop.Add(this);

            _isRunning = true;
            _remainTime = time;
            _timeOutHandler = timeOutHandler;
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _timeOutHandler = null;
            _loop.Remove(this);
        }

        public void Update()
        {
            _remainTime -= _time.GetDeltaTime();

            if (_remainTime <= 0.0f)
            {
                var handler = _timeOutHandler;
                Stop();
                handler?.Invoke();
            }
        }

        public void Destroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            if (_isRunning)
            {
                _loop.Remove(this);
                _isRunning = false;
            }
        }
    }
}
