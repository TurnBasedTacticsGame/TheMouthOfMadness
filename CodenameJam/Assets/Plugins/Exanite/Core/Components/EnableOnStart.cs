using UnityEngine;

namespace Exanite.Core.Components
{
    public class EnableOnStart : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(true);
        }
    }
}
