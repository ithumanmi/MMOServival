using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.EventObserver
{
    public class EventObs : RuntimeSingleton<EventObs>
    {
        private Dictionary<string, List<IRegister>> dictRegisters = new Dictionary<string, List<IRegister>>();

        public void AddRegister(string eventId, IRegister register)
        {
            if (!dictRegisters.TryGetValue(eventId, out var registers))
            {
                registers = new List<IRegister>();
                dictRegisters.Add(eventId, registers);
            }

            registers.Add(register);
        }

        public void RemoveRegister(string eventId, IRegister register)
        {
            if (dictRegisters.TryGetValue(eventId, out var registers))
            {
                registers.Remove(register);
            }
        }

        public void ExcuteEvent(string eventId, EventBase eventBase)
        {
#if UNITY_EDITOR
            //Debug.Log($"Excute Event : {eventId}");
#endif

            if (!dictRegisters.TryGetValue(eventId, out var registers))
            {
                return;
            }

            var newList = new List<IRegister>(registers);

            foreach (var register in newList)
            {
#if UNITY_EDITOR
                //Debug.Log($"Excute Event {eventId} to {register.GetType().Name}");
#endif
                try
                {
                    register.OnEvent(eventId, eventBase);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }

    public interface IRegister
    {
        void OnEvent(string eventId, EventBase data);
    }

    public abstract class EventBase
    {

    }
}
