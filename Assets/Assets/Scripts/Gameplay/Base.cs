using Systems.ServiceLocatorSystem;
using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    public class Base : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position - Vector3.right * 100f, transform.position + Vector3.right * 100f);
        }
    }
}