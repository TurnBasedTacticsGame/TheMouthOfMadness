using UniDi;
using UnityEngine;

namespace Source.GameEvents.Core
{
    public class GameContext : MonoBehaviour
    {
        [Inject] public EventTracker Events { get; }
    }
}
