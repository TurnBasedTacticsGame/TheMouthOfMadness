using Source.GameEvents.Core;
using Source.Players;
using UniDi;
using UnityEngine;

namespace Source.UserInterface
{
    public class PauseGameDuringSimulation : MonoBehaviour
    {
        public float transitionSpeed = 0.5f;

        [Inject] private Player player;
        [Inject] private GameContext gameContext;

        private float initialFixedDeltaTime;
        private float timeScale = 0;

        private void Awake()
        {
            initialFixedDeltaTime = Time.fixedDeltaTime;

            UpdateTimeScale();
        }

        private void Update()
        {
            var shouldPause = player.state == Player.PlayerState.Waiting && player.timeSpentWaiting > player.moveCooldown;

            var targetTimeScale = shouldPause ? 0 : 1;
            timeScale = Mathf.MoveTowards(timeScale, targetTimeScale, Time.unscaledDeltaTime * transitionSpeed);

            UpdateTimeScale();
        }

        private void UpdateTimeScale()
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = initialFixedDeltaTime * Time.timeScale;

            gameContext.IsPaused = timeScale < 0.1f;
        }
    }
}
