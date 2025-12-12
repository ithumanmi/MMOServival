using System;

namespace Hawky.ItemAnimation
{
    public class SimpleItemAnimation : IItemAnimation
    {
        private Action _excuter;

        private bool _excuted = false;

        public SimpleItemAnimation(Action excuter)
        {
            this._excuter = excuter;
        }

        public void ForceComplete()
        {
            Start();
        }

        public bool IsComplete()
        {
            return _excuted;
        }

        public bool IsStarted()
        {
            return _excuted;
        }

        public void Start()
        {
            if (IsStarted())
            {
                return;
            }
            _excuter?.Invoke();

            _excuted = true;
        }

        public void Stop()
        {
            Start();
        }
    }
}