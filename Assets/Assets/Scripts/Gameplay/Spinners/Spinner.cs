using System.Collections;
using DG.Tweening;
using FMODUnity;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    [RequireComponent(typeof(Rigidbody2D)), SelectionBase]
    public abstract class Spinner : MonoBehaviour, ISpinner, IUpdateObserver
    {
        [SerializeField, Self] protected Rigidbody2D rb;
        [SerializeField, Self] private Collider2D coll;
        protected Area workingArea;
        private Camera cam;
        [SerializeField] private SpriteRenderer rend;
        private bool clicked;
        protected Vector3 pickupPosition;


        private void OnEnable()
        {
            if (Manager.Instance != null) Manager.Instance.Register(this);
        }

        private IEnumerator Start()
        {
            cam = Camera.main;
            var size = transform.localScale.x;
            rb.bodyType = RigidbodyType2D.Static;
            coll.enabled = false;
            transform.localScale = Vector3.one * 1.5f;

            transform.DOScale(size, 0.8f).SetEase(Ease.InBack).onComplete += () =>
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                coll.enabled = true;
            };

            rend.DOFade(0.0f, 0.0f).onComplete += () => { rend.DOFade(1.0f, 0.8f); };

            yield return new WaitForEndOfFrame();
            workingArea = Manager.Instance.GetWorkingArea(transform.position);
            workingArea?.AddSpinner(this);
        }

        private void OnDisable()
        {
            if (Manager.Instance != null) Manager.Instance.Unregister(this);
        }



        private void OnMouseDown()
        {
            clicked = true;
            rb.bodyType = RigidbodyType2D.Static;
            coll.enabled = false;
            pickupPosition = transform.position;
        }

        private void OnMouseDrag()
        {
            if (!clicked) return;
            var mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            transform.position = mousePosition;
        }

        private void OnMouseUp()
        {
            clicked = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            coll.enabled = true;
            var area = Manager.Instance.GetWorkingArea(transform.position);
            if (area == null || area.locked)
            {
                RePosition();
            }
            else
            {
                workingArea = area;
                EventBus<OnSpinnerAreaChange>.Rise(this, workingArea);
                RuntimeManager.PlayOneShot("event:/Sfx/Place Spinner");
            }

            workingArea.AddSpinner(this);
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(Manager.Instance.Damage, other.contacts[0].normal);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out IDamageable enemy)) return;
            if (Time.frameCount % 15 != 0) return;
            enemy.TakeDamage(Manager.Instance.Damage, other.contacts[0].normal);
        }

        public virtual void ObserverUpdate()
        {
        }
        [ContextMenu("Shoot")]
        protected virtual void Shoot()
        {
        }
        protected virtual void RePosition()
        {
        }
    }


    public interface ISpinner
    {
    }

    public interface IDamageable
    {
        void TakeDamage(int damage, Vector2 direction);
        void Die();
    }
}