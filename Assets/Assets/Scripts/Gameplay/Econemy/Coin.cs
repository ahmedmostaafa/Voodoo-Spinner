using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using KabreetGames.BladeSpinner;
using KabreetGames.BladeSpinner.Assets.Scripts.MiniGame;
using KabreetGames.BladeSpinner.Ui;
using KabreetGames.UiSystem;
using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
    [SerializeField] private float bounceHeight = 1.5f;
    [SerializeField] private float bounceDuration = 0.45f;
    [SerializeField] private float horizontalDistance = 2f;
    [SerializeField] private int bounceCount = 4;
    private float height = 1.5f;
    private float duration = 0.5f;

    private Vector3 coinPos;

    private void Awake()
    {
        coinPos = MenuManager.GetPanel<MainPlay>().CoinWorldPosition();
    }

    public void Move(ObjectPool<Coin> pool)
    {
        height = bounceHeight;
        duration = bounceDuration;
        transform.DOMoveX(transform.position.x + horizontalDistance, duration / 2 * bounceCount)
            .SetEase(Ease.OutQuad);

        var bounceSequence = DOTween.Sequence();

        for (var i = 0; i < bounceCount; i++)
        {
            bounceSequence.Append(transform.DOMoveY(transform.position.y + height, duration / 2)
                    .SetEase(Ease.OutQuad))
                .Append(transform.DOMoveY(transform.position.y, duration / 2.5f)
                    .SetEase(Ease.InQuad));

            height *= 0.5f;
            duration *= 0.75f;
        }
        bounceSequence.AppendInterval(.4f);
        bounceSequence.Append(transform.DOMove(coinPos, .5f).SetEase(Ease.InOutFlash));
        bounceSequence.OnComplete(() =>
        {
            Manager.Instance.Coins++;
            RuntimeManager.PlayOneShot("event:/Sfx/Coin Collect");
            pool.Release(this);
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

}