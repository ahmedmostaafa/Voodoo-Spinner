using UnityEngine;

namespace KabreetGames.BladeSpinner.Assets.Scripts.MiniGame
{
    public class BounceGate : Gate
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out CoinGame coin) || !coin.CanGetThrowGate(this)) return;
            coin.Bounce();
        }
    }
}