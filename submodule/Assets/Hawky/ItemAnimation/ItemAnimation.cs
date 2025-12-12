using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.ItemAnimation
{
    public interface IItemAnimation
    {
        void Start();
        bool IsStarted();
        void ForceComplete();
        void Stop();
        bool IsComplete();
    }

    public interface IItemAnimationOwner : IGlobalTransfrom
    {

    }

    public class ItemAnimation : MonoBehaviour
    {
        private Queue<IItemAnimation> animations = new Queue<IItemAnimation>();

        public IItemAnimationOwner Owner { get; set; }

        public void AddAnimation(IItemAnimation itemAnimation)
        {
            animations.Enqueue(itemAnimation);
        }

        public bool IsFree()
        {
            return animations.Count == 0;
        }

        public void ForceCompleteAll()
        {
            foreach (var anim in animations)
            {
                anim.ForceComplete();
            }

            animations.Clear();
        }

        public void StopAll()
        {
            foreach (var anim in animations)
            {
                anim.Stop();
            }

            animations.Clear();
        }

        private void OnEnable()
        {
            StartCoroutine(InProgress());
        }

        private IEnumerator InProgress()
        {
            while (true)
            {
                while (animations.Count <= 0)
                {
                    yield return null;
                }

                var inQueue = animations.Peek();

                if (inQueue.IsComplete())
                {
                    animations.Dequeue();
                    continue;
                }

                if (!inQueue.IsStarted())
                {
                    inQueue.Start();
                }

                yield return null;
            }
        }
    }
}
