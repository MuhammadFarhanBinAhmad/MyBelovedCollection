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

    IEnumerator DisableDeathFlash()
    {
        yield return new WaitForSeconds(.5f);
        blackScreen.SetActive(false);
    }



}
