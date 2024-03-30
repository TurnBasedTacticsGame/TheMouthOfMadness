using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Source.GameEvents
{
    public abstract class EventHandler : MonoBehaviour
    {
        public virtual UniTask OnEventRaised(EventTracker tracker)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEventConfirmed(EventTracker tracker)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEventApplied(EventTracker tracker)
        {
            return UniTask.CompletedTask;
        }
    }
}
