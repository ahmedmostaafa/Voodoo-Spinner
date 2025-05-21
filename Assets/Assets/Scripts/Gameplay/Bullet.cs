using KabreetGames.SceneReferences;
using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    public class Bullet : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Rigidbody2D rb;
        [SerializeField, Child] private TrailRenderer trailRenderer;

        public void Shoot(Transform trans)
        {
            var direction = trans.right;
            transform.position = trans.position;
            trailRenderer.Clear();
            rb.linearVelocity = direction * 3;
        }

        private void Update()
        {
            if (OutBounds())
            {
                BulletSpawner.Instance.BulletPool.Release(this);
            }
        }

        private bool OutBounds()
        {
            return transform.position.sqrMagnitude > 300;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out IDamageable enemy)) return;
            enemy.TakeDamage(Manager.Instance.Damage, other.contacts[0].normal);
            BulletSpawner.Instance.BulletPool.Release(this);
        }
    }
}