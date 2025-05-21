using System;
using KabreetGames.BladeSpinner.Assets.Scripts.Gameplay;
using UnityEngine;


namespace KabreetGames.BladeSpinner
{
    public class ShootSpinner : Spinner
    {
        [SerializeField] private float shootTime = 3f;
        private float lastShootTime;
        [SerializeField] private GunShooter shootSpinner;

        private void Awake()
        {
            lastShootTime = Time.time + shootTime;
        }

        public override void ObserverUpdate()
        {
            if (ShouldShoot()) Shoot();
        }

        private bool ShouldShoot()
        {
            return (Time.time - lastShootTime > shootTime);
        }

        protected override void Shoot()
        {
            shootSpinner.Shoot();
            lastShootTime = Time.time;
        }

        protected override void RePosition()
        {
            transform.position = pickupPosition;
        }
    }
}