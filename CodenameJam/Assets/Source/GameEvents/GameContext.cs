using UniDi;
using UnityEngine;

namespace Source.GameEvents
{
    public class GameContext : MonoBehaviour
    {
        [Inject] public EventTracker Events { get; }
    }
}
