using KabreetGames.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace KabreetGames.BladeSpinner
{
    public class BulletSpawner : Singleton<BulletSpawner>
    {
        [SerializeField] private Bullet bulletPrefab;
        public ObjectPool<Bullet> BulletPool { get; set; }

        private void Awake()
        {
            BulletPool = new ObjectPool<Bullet>(() =>
                {
                    var obj = Instantiate(bulletPrefab);
                    return obj;
                },
                obj => obj.gameObject.SetActive(true),
                obj =>
                {
                    obj.transform.SetParent(transform);
                    obj.gameObject.SetActive(false);
                },
                Destroy);
        }
    }
}