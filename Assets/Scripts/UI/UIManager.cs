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



}
