using System.Collections;
using UnityEngine;

public class UIManager : UISubject
{

    [Header("RestartBlackScreen")]
    [SerializeField] GameObject blackScreen;

    public void UpdateEnemyCount(BaseEnemy enemy)
    {
        print("enemy down");
        enemy.OnEnemyDied -= UpdateEnemyCount;

    }

    public void DeathFlashScreem()
    {
        blackScreen.SetActive(true);
        StartCoroutine(DisableDeathFlash());
    }

    public void MoveNextRoomFlash()
    {
        blackScreen.SetActive(true);
        StartCoroutine(MoveNextRoomFlashCoroutine());
    }

    IEnumerator DisableDeathFlash()
    {
        yield return new WaitForSeconds(.5f);
        blackScreen.SetActive(false);
        PlayerManager.Instance.SetIsDead(false);
    }
    IEnumerator MoveNextRoomFlashCoroutine()
    {
        yield return new WaitForSeconds(.1f);
        blackScreen.SetActive(false);
    }



}
