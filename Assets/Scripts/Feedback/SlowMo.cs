using UnityEngine;
using DG.Tweening;

public class SlowMo : MonoBehaviour
{
    public void AddSlowmoEffect(BaseEnemy be) => be.OnEnemyDied += SlowMoEffect;

    private void SlowMoEffect<T>(T sender)
    {
        // Avoid multiple triggers if one is already active
        DOTween.Kill("SlowMoTween");

        // Start slow motion
        float originalScale = Time.timeScale;
        float targetScale = 0.1f; // how slow (20% speed)
        float slowDuration = 0.3f; // real-time duration of the effect

        // Smoothly go into slowmo
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, targetScale, 0f)
            .SetUpdate(true) // ensures it runs even when timescale is slowed
            .SetId("SlowMoTween")
            .OnComplete(() =>
            {
                // Wait in real-time, then restore
                DOVirtual.DelayedCall(slowDuration, () =>
                {
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, originalScale, 0.15f)
                        .SetUpdate(true)
                        .SetId("SlowMoTween");
                }).SetUpdate(true);
            });
    }

}
