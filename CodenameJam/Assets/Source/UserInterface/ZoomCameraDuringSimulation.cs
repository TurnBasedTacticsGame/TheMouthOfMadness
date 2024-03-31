using Cinemachine;
using Source.GameEvents.Core;
using UniDi;
using UnityEngine;

namespace Source.UserInterface
{
    public class ZoomCameraDuringSimulation : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;

        public float pausedZoom = 8;
        public float simulationZoom = 10;
        public float smoothTime = 0.5f;

        [Inject] private GameContext gameContext;
        [Inject] private Camera mainCamera;

        private float currentZoomVelocity;

        private void Update()
        {
            var targetZoom = gameContext.IsPaused ? pausedZoom : simulationZoom;
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetZoom, ref currentZoomVelocity, smoothTime, float.PositiveInfinity, Time.unscaledDeltaTime);
            mainCamera.orthographicSize = virtualCamera.m_Lens.OrthographicSize;
        }
    }
}
