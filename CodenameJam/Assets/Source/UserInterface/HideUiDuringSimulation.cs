using Source.Players;
using UniDi;
using UnityEngine;

namespace Source.UserInterface
{
    public class HideUiDuringSimulation : MonoBehaviour
    {
        public RectTransform hiddenPosition;
        public float speed = 10;

        [Inject] private Player player;

        /// <summary>
        /// Tracks the initial location.
        /// </summary>
        private RectTransform shownPosition;

        private RectTransform rectTransform => (RectTransform)transform;

        private void Start()
        {
            shownPosition = new GameObject($"{GetType().Name}-ShownPosition").AddComponent<RectTransform>();
            shownPosition.position = rectTransform.position;
        }

        private void Update()
        {
            var targetPosition = player.state == Player.PlayerState.Moving ? hiddenPosition : shownPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.unscaledDeltaTime * speed);
        }
    }
}
