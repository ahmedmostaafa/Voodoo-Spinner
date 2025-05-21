using DG.Tweening;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Assets.Scripts.Gameplay
{
    public class GunShooter : MonoBehaviour
    {
        private float lastShootTime;
        [SerializeField] private int shootCount = 7;

        [SerializeField] private float shootTime = 0.5f;
        private float hitTime;

        private void Awake()
        {
            lastShootTime = Time.time;
            hitTime = shootTime / shootCount;
        }

        public void Shoot()
        {
            transform.DORotate(new Vector3(0, 0, 360), shootTime, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).onUpdate += () =>
            {
                if (!(Time.time - lastShootTime > hitTime)) return;
                lastShootTime = Time.time;
                SpawnShoot();
            };
        }

        private void SpawnShoot()
        {
            var bullet = BulletSpawner.Instance.BulletPool.Get();
            bullet.Shoot(transform);
        }
    }
}