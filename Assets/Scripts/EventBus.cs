using System;
using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios.Utility.Events
{
    public static class EventBus<T> where T : IEvent
    {
        private static HashSet<IEventBinding<T>> _bindings = new HashSet<IEventBinding<T>>();

        public static void RegisterBinding(IEventBinding<T> binding)
        {
            _bindings.Add(binding);
        }

        public static void UnregisterBinding(IEventBinding<T> binding)
        {
            _bindings.Remove(binding);
        }

        public static void RaiseEvent(T @event)
        {
            foreach (IEventBinding<T> binding in _bindings)
            {
                binding.OnEvent(@event);
                binding.OnEventNoArgs();
            }
        }
    }
    
    public interface IEvent{}

    public interface IEventBinding<T>
    {
        Action OnEventNoArgs { get; set; }
        Action<T> OnEvent { get; set; }
    }

    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> onEvent = _ => { };
        private Action onEventNoArgs = () => { };
        
        public Action<T> OnEvent {get => onEvent; set => onEvent = value; }
        public Action OnEventNoArgs {get => onEventNoArgs; set => onEventNoArgs = value; }
        
        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;
        
        public void AddEvent(Action<T> @event) => onEvent += @event;
        public void AddEvent(Action @event) => onEventNoArgs += @event;
    }
}
