using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.EventObserver
{
    public abstract class EventBehaviour : MonoBehaviour, IRegister
    {
        private List<string> eventList = new List<string>();

        protected virtual void OnEnable()
        {
            eventList.Clear();
            EventList(eventList);

            foreach (var eventName in eventList)
            {
                EventObs.Ins.AddRegister(eventName, this);
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var eventName in eventList)
            {
                EventObs.Ins.RemoveRegister(eventName, this);
            }
        }

        protected abstract void EventList(List<string> eventList);

        public abstract void OnEvent(string eventName, EventBase data);
    }
}