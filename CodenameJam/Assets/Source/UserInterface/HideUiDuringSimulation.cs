using Source.GameEvents.Core;
using UniDi;
using UnityEngine;

namespace Source.UserInterface
{
    public class HideUiDuringSimulation : MonoBehaviour
    {
        public RectTransform hiddenPosition;
        public float speed = 10;

        [Inject] private GameContext gameContext;

        /// <summary>
        /// Tracks the initial location.
        /// </summary>
        private RectTransform shownPosition;

        private RectTransform RectTransform => (RectTransform)transform;

        private void Start()
        {
            shownPosition = new GameObject($"{GetType().Name}-ShownPosition").AddComponent<RectTransform>();
            shownPosition.position = RectTransform.position;
        }

        private void Update()
        {
            var targetPosition = gameContext.IsPaused ? shownPosition : hiddenPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.unscaledDeltaTime * speed);
        }
    }
}
