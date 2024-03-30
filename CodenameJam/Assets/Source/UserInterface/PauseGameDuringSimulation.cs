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

        private float timeScale = 0;

        private void Awake()
        {
            initialFixedDeltaTime = Time.fixedDeltaTime;

            UpdateTimeScale();
        }

        private void Update()
        {
            var targetTimeScale = player.state == Player.PlayerState.Moving ? 1 : 0;
            timeScale = Mathf.SmoothDamp(timeScale, targetTimeScale, ref currentSmoothVelocity, smoothTime, float.PositiveInfinity, Time.unscaledDeltaTime);

            UpdateTimeScale();
        }

        private void UpdateTimeScale()
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = initialFixedDeltaTime * Time.timeScale;
        }
    }
}
