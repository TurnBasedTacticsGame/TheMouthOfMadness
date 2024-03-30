using Cinemachine;
using Source.Players;
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

        [Inject] private Player player;

        private float currentZoomVelocity;

        private void Update()
        {
            var targetZoom = player.state == Player.PlayerState.Moving ? simulationZoom : pausedZoom;
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetZoom, ref currentZoomVelocity, smoothTime, float.PositiveInfinity, Time.unscaledDeltaTime);
        }
    }
}
