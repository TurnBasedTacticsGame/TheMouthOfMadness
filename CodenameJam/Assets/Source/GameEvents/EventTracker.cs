using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniDi;
using UnityEngine;

namespace Source.GameEvents
{
    public class EventTracker : MonoBehaviour
    {
        [Inject] public List<EventHandler> EventHandlers { get; } = new();

        public List<GameEvent> Stack { get; } = new();
        public int CurrentEventIndex => Stack.Count - 1;

        public async UniTask RaiseEvent(GameEvent gameEvent)
        {
            Stack.Add(gameEvent);
            {
                await OnEventRaised(gameEvent);
                await OnEventConfirmed(gameEvent);
                await gameEvent.Apply(this);
                await OnEventApplied(gameEvent);
            }
            Stack.RemoveAt(CurrentEventIndex);
        }
        
        private async UniTask OnEventRaised(GameEvent gameEvent)
        {
            foreach (var eventHandler in EventHandlers)
            {
                await eventHandler.OnEventRaised(this);
            }
        }

        private async UniTask OnEventConfirmed(GameEvent gameEvent)
        {
            foreach (var eventHandler in EventHandlers)
            {
                await eventHandler.OnEventConfirmed(this);
            }
        }

        private async UniTask OnEventApplied(GameEvent gameEvent)
        {
            foreach (var eventHandler in EventHandlers)
            {
                await eventHandler.OnEventApplied(this);
            }
        }
    }
}
