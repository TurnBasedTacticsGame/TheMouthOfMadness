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

        public UniTask RaiseEvent(GameEvent gameEvent)
        {
            return UniTask.CompletedTask;
        }
    }
}
