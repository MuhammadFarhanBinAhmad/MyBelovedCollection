using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    [SerializeField] private Image _flash;

    // Generic helper to add event listeners for different object types
    public void AddCameraFlashEvent(BaseEnemy enemy)
        => enemy.OnEnemyDied += HitFlash<BaseEnemy>;

    public void AddCameraFlashEvent(PlayerManager pm)
        => pm.OnPlayerDied += HitFlash<PlayerManager>;

    public void AddCameraFlashEvent(DestructableProjectiles dp)
        => dp.OnProjectileHit += HitFlash<DestructableProjectiles>;

    // Generic HitFlash method
    private void HitFlash<T>(T sender)
    {
        // Try unsubscribing if sender has a known event type
        switch (sender)
        {
            case BaseEnemy enemy:
                enemy.OnEnemyDied -= HitFlash<BaseEnemy>;
                break;
            case PlayerManager player:
                player.OnPlayerDied -= HitFlash<PlayerManager>;
                break;
            case DestructableProjectiles destructableProjectiles:
                destructableProjectiles.OnProjectileHit -= HitFlash<DestructableProjectiles>;
                break;
        }

        if (_flash == null) return;

        // Flash effect
        _flash.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _flash.gameObject.SetActive(false);
        });
    }
}
