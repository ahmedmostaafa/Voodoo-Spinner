using System;
using KabreetGames.SceneReferences;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Assets.Scripts.MiniGame
{
    public class Collect : ValidatedMonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out CoinGame coin)) return;
            CoinsPool.Instance.Release(coin);
        }
    }
}