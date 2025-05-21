using KabreetGames.SceneReferences;
using Systems.ServiceLocatorSystem;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Service
{
    public class CameraService : MonoBehaviour
    {
        [SerializeField, Self] private Camera cam;
        public Camera Camera => cam;
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this);
        }
        
    }
}  