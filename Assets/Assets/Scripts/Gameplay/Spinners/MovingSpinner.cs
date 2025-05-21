using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    public class MovingSpinner : Spinner
    {
        private void Awake()
        {
            Shoot();
        }
        
        public override void ObserverUpdate()
        {
            if (ShouldBoost()) BoostSpeed();

            if (ShouldShoot()) Shoot();
            if (ShouldRecover()) RePosition();
        }

        protected override void Shoot()
        {
            if (rb.bodyType != RigidbodyType2D.Dynamic) return;
            var random = Random.Range(-1f, 1f);
            var forceDirection = new Vector2(random, random).normalized;
            rb.linearVelocity = forceDirection * Manager.Instance.Force;
        }
        
        protected override void RePosition()
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = workingArea.transform.position;
            Shoot();
        }

        private bool ShouldRecover()
        {
            if (workingArea == null || rb.bodyType != RigidbodyType2D.Dynamic) return false;
            return !Manager.InsideAreaFrame(transform.position, workingArea);
        }

        private bool ShouldShoot()
        {
            return Mathf.Abs(rb.linearVelocity.x) < 0.01f || Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }

        private bool ShouldBoost()
        {
            if (rb.bodyType != RigidbodyType2D.Dynamic) return false;
            var force = Manager.Instance.Force;
            return rb.linearVelocity.sqrMagnitude < (force * force * 0.9f) ||
                   rb.linearVelocity.sqrMagnitude > (force * force * 1.1f);
        }

        private void BoostSpeed()
        {
            var direction = rb.linearVelocity.normalized;
            rb.linearVelocity = direction * Manager.Instance.Force;
        }
    }
}