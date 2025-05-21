using UnityEngine;
using UnityEngine.Pool;

namespace KabreetGames.BladeSpinner
{
    public class SpinnerSpawner : MonoBehaviour
    {
        [SerializeField] private MovingSpinner spinnerPrefab;
        [SerializeField] private ShootSpinner shootSpinnerPrefab;
        private ObjectPool<MovingSpinner> SpinnerPool { get; set; }
        private ObjectPool<ShootSpinner> ShootSpinnerPool { get; set; }


        private void Awake()
        {
            SpinnerPool = new ObjectPool<MovingSpinner>(() =>
                {
                    var obj = Instantiate(spinnerPrefab);
                    return obj;
                },
                obj => obj.gameObject.SetActive(true),
                obj =>
                {
                    obj.transform.SetParent(transform);
                    obj.gameObject.SetActive(false);
                },
                Destroy);
            ShootSpinnerPool = new ObjectPool<ShootSpinner>(() =>
                {
                    var obj = Instantiate(shootSpinnerPrefab);
                    return obj;
                },
                obj => obj.gameObject.SetActive(true),
                obj =>
                {
                    obj.transform.SetParent(transform);
                    obj.gameObject.SetActive(false);
                },
                Destroy);


            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnDestroy()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }


        private void OnEnable()
        {
            if (Manager.Instance == null) return;
            Manager.Instance.OnWaveUpgradeEvent += SpawnSpinner;
            Manager.Instance.OnWaveUpgradeEvent += SpawnShootSpinner;
        }

        private void OnDisable()
        {
            if (Manager.Instance == null) return;
            Manager.Instance.OnWaveUpgradeEvent -= SpawnSpinner;
            Manager.Instance.OnWaveUpgradeEvent -= SpawnShootSpinner;
        }

        private void SpawnSpinner(WaveUpgradeEvent waveUpgradeEvent)
        {
            if (waveUpgradeEvent.waveUpgradeEventName != nameof(SpawnSpinnerEvent)) return;
            var spinner = SpinnerPool.Get();
            var randomArea = Manager.Instance.GetRandomArea();
            spinner.transform.position = randomArea.transform.position;
        }

        private void SpawnShootSpinner(WaveUpgradeEvent waveUpgradeEvent)
        {
            if (waveUpgradeEvent.waveUpgradeEventName != nameof(SpawnShootSpinnerEvent)) return;
            var spinner = ShootSpinnerPool.Get();
            var randomArea = Manager.Instance.GetRandomArea();
            spinner.transform.position = randomArea.transform.position;
        }
    }


    public abstract class SpawnSpinnerEvent : WaveUpgradeEvent
    {
        protected SpawnSpinnerEvent(string waveUpgradeEventName) : base(waveUpgradeEventName)
        {
        }
    }

    public abstract class SpawnShootSpinnerEvent : WaveUpgradeEvent
    {
        protected SpawnShootSpinnerEvent(string waveUpgradeEventName) : base(waveUpgradeEventName)
        {
        }
    }
}