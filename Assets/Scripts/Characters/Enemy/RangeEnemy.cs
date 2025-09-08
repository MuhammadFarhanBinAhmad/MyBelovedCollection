using UnityEngine;

public class RangeEnemy : BaseEnemy
{
    [SerializeField] SO_ProjectileStats so_projectileStats;

    ProjectilePool _pool;
    [SerializeField] Transform proj_SpawnPos;

    [SerializeField] Transform _weapon;

    private void Start()
    {
        _pool = FindAnyObjectByType<ProjectilePool>();
    }

    public override void AttackPlayer()
    {
        _weapon.right = _target.transform.position - transform.position;

        if (Time.time >= _nextAttack)
        {
            GameObject GO_proj = _pool.GetProjectile();
            Projectiles _proj = GO_proj.GetComponent<Projectiles>();

            Vector2 direction = (_target.transform.position - proj_SpawnPos.position).normalized;

            _proj.SetOwner(BULLETOWNER.ENEMY);
            _proj.SetDirection(direction);
            _proj.SetPosition(proj_SpawnPos.position);
            _proj.SetDamage(so_projectileStats._damage);
            _proj.SetSpeed(so_projectileStats._speed);

            _nextAttack = Time.time + _attackRate; // schedule next attack }
        }
    }

}
