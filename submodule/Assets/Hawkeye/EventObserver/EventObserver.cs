using System.Collections.Generic;
using UnityEngine;

namespace Hawkeye.EventObserver
{

    public class EventObserver : Singleton<EventObserver>
    {
        private Dictionary<string, List<IRegister>> dictRegisters = new Dictionary<string, List<IRegister>>();

        public void AddRegister(string eventId, IRegister register)
        {
            if (!dictRegisters.TryGetValue(eventId, out var registers))
            {
                registers = new List<IRegister>();
                dictRegisters.Add(eventId, registers);
            }

#if UNITY_EDITOR
            Debug.Log($"Add Event : {eventId} with {register.GetType().Name}");
#endif

            registers.Add(register);
        }

        public void RemoveRegister(string eventId, IRegister register)
        {
            if (dictRegisters.TryGetValue(eventId, out var registers))
            {

#if UNITY_EDITOR
                Debug.Log($"Remove Event : {eventId} with {register.GetType().Name}");
#endif

                registers.Remove(register);
            }
        }

        public void ExcuteEvent(string eventId, EventBase eventBase)
        {
#if UNITY_EDITOR
            Debug.Log($"Excute Event : {eventId}");
#endif

            if (!dictRegisters.TryGetValue(eventId, out var registers))
            {
                return;
            }

            var newList = new List<IRegister>(registers);

            foreach (var register in newList)
            {
#if UNITY_EDITOR
                Debug.Log($"Excute Event {eventId} to {register.GetType().Name}");
#endif

                register.OnEvent(eventId, eventBase);
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
