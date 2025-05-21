using System.Collections.Generic;
using FMODUnity;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using KabreetGames.EventBus.Interfaces;
using Systems.ServiceLocatorSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KabreetGames.BladeSpinner.TowerSystem
{
    public class Tower : MonoBehaviour, IDamageable, IFixedUpdateObserver
    {
        private readonly Stack<Plate> plates = new();
        private TowerSpawner towerSpawner;
        [SerializeField] private float speed = 1f;
        [SerializeField] private Color[] colors;
        private Base baseObject;

        private void Start()
        {
            baseObject = ServiceLocator.ForSceneOf(this).Get<Base>();
        }

        private void OnEnable()
        {
            if (Manager.Instance != null) Manager.Instance.Register(this);
        }

        private void OnDisable()
        {
            if (Manager.Instance != null) Manager.Instance.Unregister(this);
        }

        public void ObserverFixedUpdate()
        {
            if (baseObject == null) return;
            transform.position += Vector3.down * (speed * Time.deltaTime);
            if (!(transform.position.y < baseObject.transform.position.y)) return;
            towerSpawner.TowerPool.Release(this);
            var value = WaveUpgradeData.Instance.GetStat<int>("Health");
            value.AddModifier(new AddStatModifier(-1));
            RuntimeManager.PlayOneShot("event:/Sfx/Health Lose");
        }

        public void SetUp(TowerSpawner spawner, float slotWidth, int maxHitsPower, Color randomColor = default)
        {
            towerSpawner = spawner;
            SpawnPlate(maxHitsPower, randomColor);
            transform.localScale = Vector3.one * slotWidth;
        }

        private int totalHealth;

        private void SpawnPlate(int power, Color randomColor)
        {
            if (randomColor == default) randomColor = colors[Random.Range(0, colors.Length)];
            totalHealth =
                Mathf.CeilToInt(Mathf.Pow(2, power));
            var plateCount = Mathf.Max(1, power * 2);

            for (var i = 0; i < plateCount; i++)
            {
                var plate = towerSpawner.PlatePool.Get();
                plate.transform.SetParent(transform);
                plate.transform.position = transform.position;
                plate.transform.rotation = Quaternion.identity;
                plate.SetUp(i, totalHealth, randomColor);
                plates.Push(plate);
            }
        }

        public void TakeDamage(int damage, Vector2 direction)
        {
            if (gameObject == null || !gameObject.activeSelf) return;

            totalHealth -= damage;
            totalHealth = Mathf.Max(0, totalHealth);

            var plateCount = plates.Count;
            int platesToRemove;

            if (plateCount == 1 && totalHealth > 0)
            {
                platesToRemove = 0;
            }
            else
            {
                var healthRatio = totalHealth > 0 ? (damage / (float)(totalHealth + damage)) : 1f;
                platesToRemove = Mathf.CeilToInt(healthRatio * plateCount);
            }

            platesToRemove = Mathf.Min(platesToRemove, plates.Count);

            for (var i = 0; i < platesToRemove; i++)
            {
                if (plates.Count == 0) break;
                var plate = plates.Pop();
                towerSpawner.PlatePool.Release(plate);
            }


            if (totalHealth <= 0 || plates.Count == 0)
            {
                Die();
                EventBus<OnTowerDie>.Rise(1);
            }
            else
            {
                plates.Peek().UpdateDamage(totalHealth);
            }
        }


        public void Die()
        {
            var shouldDrop = Random.Range(0f, 1f) <= 0.5f;
            //Add Drop Count logic here based on state chance
            
            if (shouldDrop)
            {
                var coins = towerSpawner.CoinPool.Get();
                coins.transform.position = transform.position;
                coins.transform.rotation = Quaternion.identity;
                coins.Move(towerSpawner.CoinPool);
            }

            RuntimeManager.PlayOneShot("event:/Sfx/Tower Hit");
            towerSpawner.TowerPool.Release(this);
        }

        public void Release()
        {
            foreach (var plate in plates)
            {
                towerSpawner.PlatePool.Release(plate);
            }

            plates.Clear();
            towerSpawner = null;
        }

        public Color GetColor(int i)
        {
            i = Mathf.Clamp(i, 0, colors.Length - 1);
            return colors[i];
        }
    }
}