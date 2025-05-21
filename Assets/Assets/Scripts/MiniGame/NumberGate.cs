using UnityEngine;

namespace KabreetGames.BladeSpinner.Assets.Scripts.MiniGame
{
    public class NumberGate : Gate
    {
        [SerializeField, Min(0)] private int op;
        [SerializeField, Min(0)] private int value;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out CoinGame coin) || !coin.CanGetThrowGate(this)) return;
        
            var rb = coin.GetComponent<Rigidbody2D>();
            var velocity = rb.linearVelocity;
            var newVelocity = (velocity + Vector2.right * 0.2f).normalized * velocity.magnitude;
            rb.linearVelocity = newVelocity;
            var otherVelocity = (velocity - Vector2.right * 0.2f).normalized * velocity.magnitude;
            switch (op)
            {
                case 0:
                    var i = 0;
                    for (; i < value; i++)
                    {
                        var newCoin= CoinsPool.Instance.GetCoin();
                        newCoin.transform.position = coin.transform.position + Vector3.right * 0.2f;
                        newCoin.SetVelocity( otherVelocity);
                        newCoin.CopyGate(coin);
                    }
                    break;
                case 1:
                    break;
            }
        }
    }
}