using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace KabreetGames.BladeSpinner.TowerSystem
{
    public class TowerSpawner : MonoBehaviour
    {
        [SerializeField] private Tower towerPrefab;
        [SerializeField] private Coin coinPrefab;
        [SerializeField] private Plate platePrefab;
        public ObjectPool<Plate> PlatePool { get; private set; }
        public ObjectPool<Tower> TowerPool { get; private set; }
        public ObjectPool<Coin> CoinPool { get; private set; }
        private bool bossSpawned;

        private readonly bool canSpawn = true;

        private void Awake()
        {
            PlatePool = new ObjectPool<Plate>(() =>
                {
                    var plate = Instantiate(platePrefab);
                    plate.gameObject.SetActive(false);
                    return plate;
                },
                plate =>
                {
                    plate.transform.SetParent(null);
                    plate.gameObject.SetActive(true);
                },
                plate =>
                {
                    plate.transform.SetParent(transform);
                    plate.gameObject.SetActive(false);
                },
                plate => Destroy(plate.gameObject));

            TowerPool = new ObjectPool<Tower>(() =>
                {
                    var tower = Instantiate(towerPrefab);
                    return tower;
                },
                tower =>
                {
                    tower.transform.SetParent(null);
                    tower.gameObject.SetActive(true);
                },
                tower =>
                {
                    tower.Release();
                    tower.transform.SetParent(transform, true);
                    tower.gameObject.SetActive(false);
                },
                tower => Destroy(tower.gameObject));

            CoinPool = new ObjectPool<Coin>(() =>
                {
                    var coin = Instantiate(coinPrefab);
                    return coin;
                },
                coin =>
                {
                    coin.transform.SetParent(null);
                    coin.gameObject.SetActive(true);
                },
                coin =>
                {
                    coin.transform.SetParent(transform, true);
                    coin.gameObject.SetActive(false);
                },
                coin => Destroy(coin.gameObject));
        }

        private IEnumerator Start()
        {
            while (canSpawn)
            {
                SpawnTower();
                yield return new WaitForSeconds(1f);
            }
        }

        private void SpawnTower()
        {
            var waveNumber = Manager.Instance.WaveNumber.Value;
            var availableSlots = new List<int> { 0, 1, 2, 3, 4, 5 };
            var slotWidth = transform.lossyScale.x / 6f;
            if (waveNumber == 15 && !bossSpawned)
            {
                SpawnBossCluster();
                bossSpawned = true;
                return;
            }

            var maxTowers = Mathf.Min(Mathf.CeilToInt(waveNumber / 2f), 3);
            if (maxTowers > 6) maxTowers = 6;
            var numToSpawn = Random.Range(0, maxTowers + 1);

            for (var i = 0; i < numToSpawn; i++)
            {
                var randomIndex = Random.Range(0, availableSlots.Count);
                var xPos = availableSlots[randomIndex] - 2.5f;
                xPos *= slotWidth;
                availableSlots.RemoveAt(randomIndex);
                var tower = TowerPool.Get();
                var position = transform.position + Vector3.right * xPos;
                tower.transform.position = position;
                var maxHitsPower = Mathf.CeilToInt(Mathf.Clamp(waveNumber / 2f, 1, 9));
                var power = Mathf.CeilToInt(Random.Range(maxHitsPower - 2, maxHitsPower + 1));
                if (power < 0) power = 0;
                tower.SetUp(this, slotWidth, power);
            }
        }

        private void SpawnBossCluster()
        {
            var slotWidth = transform.lossyScale.x / 6f;

            var rowOffset = slotWidth * 0.8f;
            var colOffset = slotWidth * 0.8f;
            var maxDistance = Mathf.Sqrt(2);

            var center = transform.position;

            for (var row = -1; row <= 1; row++)
            {
                for (var col = -1; col <= 1; col++)
                {
                    var spawnPos = center + new Vector3(col * colOffset, row * rowOffset, 0);
                    var bossTower = TowerPool.Get();
                    bossTower.transform.position = spawnPos;
                    var distance = Mathf.Sqrt(row * row + col * col);
                    var c = Mathf.RoundToInt(Mathf.Lerp(0, 2, distance / maxDistance));
                    var color = bossTower.GetColor(c);
                    var maxHitsPower = Mathf.RoundToInt(Mathf.Lerp(6, 4, distance / maxDistance));
                    bossTower.SetUp(this, slotWidth, maxHitsPower, color);
                }
            }
        }
    }
}