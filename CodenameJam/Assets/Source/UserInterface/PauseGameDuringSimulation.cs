using Source.Players;
using UniDi;
using UnityEngine;

namespace Source.UserInterface
{
    public class PauseGameDuringSimulation : MonoBehaviour
    {
        public float smoothTime = 0.5f;

        [Inject] private Player player;

        private float currentSmoothVelocity;
        private float initialFixedDeltaTime;

        private void Start()
        {
            initialFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Update()
        {
            var targetTimeScale = player.state == Player.PlayerState.Moving ? 1 : 0;
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref currentSmoothVelocity, smoothTime, float.PositiveInfinity, Time.unscaledDeltaTime);
            Time.fixedDeltaTime = initialFixedDeltaTime * Time.timeScale;
        }
    }
}
