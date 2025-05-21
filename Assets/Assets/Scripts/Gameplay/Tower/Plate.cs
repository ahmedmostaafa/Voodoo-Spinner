using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace KabreetGames.BladeSpinner.TowerSystem
{
    [RequireComponent(typeof(SortingGroup))]
    public class Plate : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private const float Step = 0.075f;
        private SortingGroup sortingGroup;


        private void Awake()
        {
            sortingGroup = GetComponent<SortingGroup>();
        }

        public void SetUp( int index,int totalHealth, Color randomColor)
        {
            var position = transform.localPosition;
            position.y = index * Step;
            transform.localPosition = position;
            sortingGroup.sortingOrder = index;
            transform.localScale = Vector3.one;
            spriteRenderer.color = randomColor;
            text.text = totalHealth.ToString();
        }
        

        public void UpdateDamage(int totalHealth)
        {
            text.text = totalHealth.ToString();
        }
    }
}