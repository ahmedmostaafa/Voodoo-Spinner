namespace KabreetGames.BladeSpinner
{
    using UnityEngine;

    public class Rotator : MonoBehaviour, IUpdateObserver
    {
        [SerializeField] private float speed = 10f;
        
        public void OnEnable()
        {
            if (Manager.Instance != null) Manager.Instance.Register(this);
        }

        public void OnDisable()
        {
            if (Manager.Instance != null) Manager.Instance.Unregister(this);
        }

        public void ObserverUpdate()
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}