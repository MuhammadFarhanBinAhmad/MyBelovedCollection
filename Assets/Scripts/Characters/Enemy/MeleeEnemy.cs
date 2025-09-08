using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    public override void AttackPlayer()
    {
        if (Time.time >= _nextAttack)
        {
            Debug.Log("attack player"); _nextAttack = Time.time + _attackRate; // schedule next attack }
        }
    }
}
