using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniDi;
using UnityEngine;

namespace Source.GameEvents.Core
{
    public class EventTracker : MonoBehaviour
    {
        [Inject] public List<EventHandler> EventHandlers { get; } = new();

        public List<GameEventNode> Stack { get; } = new();
        public int CurrentEventIndex => Stack.Count - 1;
        public GameEventNode CurrentEvent => Stack[CurrentEventIndex];

        public async UniTask RaiseEvent(GameEvent gameEvent)
        {
            var node = new GameEventNode(gameEvent);
            Stack.Add(node);
            {
                await OnEventRaised(gameEvent);
                node.Lock();
                await OnEventConfirmed(gameEvent);
                await gameEvent.Apply(this);
                await OnEventApplied(gameEvent);
            }
            Stack.RemoveAt(CurrentEventIndex);
            await OnEventFinished(gameEvent);
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

        private async UniTask OnEventFinished(GameEvent gameEvent)
        {
            foreach (var eventHandler in EventHandlers)
            {
                await eventHandler.OnEventFinished(this);
            }
        }
    }
}
