using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    public Image _flash;


    public void AddCameraFlashEvent(BaseEnemy enemy) => enemy.OnEnemyDied += HitFlash;
    void HitFlash(BaseEnemy enemy)
    {

        enemy.OnEnemyDied -= HitFlash;
        if (_flash == null) return;

        // Freeze the game
        //Time.timeScale = .1f;

        // Wait 0.5s in real-time, then resume time and show flash
        //DOVirtual.DelayedCall(0.5f, () =>
        //{
        //    //Time.timeScale = 1f; // Resume

        //    _flash.gameObject.SetActive(true);

        //    // Show flash for 0.2s
        //    DOVirtual.DelayedCall(0.2f, () =>
        //    {
        //        _flash.gameObject.SetActive(false);
        //    });
        //}).SetUpdate(true); // ensures this runs in real-time, ignoring timeScale

        _flash.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            _flash.gameObject.SetActive(false);
        });
    }
     
}
