using UnityEngine;

public class Player_Combo : MonoBehaviour
{
    int combo_Count;

    public void AddEnemyToComboCountList(BaseEnemy enemy)
    {
        enemy.OnEnemyHit += IncreaseComboCount;
        enemy.OnEnemyDied += IncreaseComboCount;
    }

    public void IncreaseComboCount(BaseEnemy enemy)
    {
        combo_Count++;
        UI_PlayerCombo.Instance.ChangeComboNumberLevelText(combo_Count);
    }
}
