using KabreetGames.SceneReferences;
using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    public class Area : MonoBehaviour
    {
        private static readonly int LockedHash = Shader.PropertyToID("_Locked");
        public Vector2 Size { get; private set; }
        public bool locked;
        [SerializeField] private GameObject child;

        [SerializeField, Self] private SpriteRenderer spriteRenderer;
        [SerializeField] private int openCost = 30;

        private void Awake()
        {
            Size = spriteRenderer.localBounds.size;
            var propertyBlock = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInt(LockedHash, locked ? 1 : 0);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }


        public void AddSpinner(ISpinner spinner)
        {
        }

        private void OnMouseDown()
        {
            if (!locked || Manager.Instance.Coins < openCost) return;
            locked = false;
            var propertyBlock = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInt(LockedHash, locked ? 1 : 0);
            spriteRenderer.SetPropertyBlock(propertyBlock);
            child.SetActive(false);
            Manager.Instance.Coins -= openCost;

            // MenuManager.OpenPanel<AreaUnlock>();
            // EventBus<OnAreaClicked>.Rise(this);
        }
    }
}