using KabreetGames.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace KabreetGames.BladeSpinner.Assets.Scripts.MiniGame
{
    public class CoinsPool : Singleton<CoinsPool>
    {
        [SerializeField] private CoinGame coinPrefab;
        [SerializeField] private int startSize;

        private ObjectPool<CoinGame> pool;

        protected override void OnAwake()
        {
            pool = new ObjectPool<CoinGame>(
                () =>
                {
                    var coin = Instantiate(coinPrefab, transform);
                    coin.gameObject.SetActive(false);
                    return coin;
                },
                coin =>
                {
                    coin.gameObject.SetActive(true);
                    coin.ResetData();
                },
                coin => coin.gameObject.SetActive(false),
                Destroy, false, defaultCapacity: startSize, maxSize: startSize);

            var l = new CoinGame[startSize];
            for (var i = 0; i < startSize; i++)
            {
                var p = pool.Get();
                l[i] = p;
            }

            for (var i = 0; i < startSize; i++)
            {
                pool.Release(l[i]);
            }

        }

        public CoinGame GetCoin() => pool.Get();

        public void Release(CoinGame component)
        {
            pool.Release(component);
        }
    }
}