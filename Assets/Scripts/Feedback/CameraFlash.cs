using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    public Image _flash;


    public void AddCameraFlashEvent(BaseEnemy enemy) => enemy.OnEnemyDied += HitFlash;
    public void AddCameraFlashEvent(PlayerManager pm) => pm.OnPlayerDied += HitFlash;

    void HitFlash(BaseEnemy enemy)
    {

        enemy.OnEnemyDied -= HitFlash;
        if (_flash == null) return;

        _flash.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _flash.gameObject.SetActive(false);
        });
    }

    void HitFlash(PlayerManager pm)
    {
        if (_flash == null) return;

        _flash.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _flash.gameObject.SetActive(false);
        });
    }



}
