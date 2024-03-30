using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Source.GameEvents
{
    public class EventTracker : MonoBehaviour
    {
        public UniTask RaiseEvent(GameEvent gameEvent)
        {
            return UniTask.CompletedTask;
        }
    }
}
