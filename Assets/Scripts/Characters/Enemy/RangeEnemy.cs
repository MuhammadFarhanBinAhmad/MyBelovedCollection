using Unity.VisualScripting;
using UnityEngine;


public class RangeEnemy : BaseEnemy
{
    [SerializeField] SO_ProjectileStats so_projectileStats;

    [SerializeField] BULLETOWNER enum_BulletOwner;

    ProjectilePool _pool;
    [SerializeField] Transform proj_SpawnPos;

    [SerializeField] Transform _weapon;

    [SerializeField] bool _isStaticEnemy;

    private void Start()
    {
        _pool = FindAnyObjectByType<ProjectilePool>();
    }
    public override void DetectPlayer()
    {
        if (_target == null)
            _target = PlayerManager.Instance.gameObject;

        float distanceToPlayer = Vector2.Distance(transform.position, _target.transform.position);

        // Detection purely by distance
        if (distanceToPlayer <= _viewRange)
        {
            _state = STATE.ATTACKING;
        }
    }
    public override void ChasePlayer()
    {
        if (_target == null)
            _target = PlayerManager.Instance.gameObject;


        Vector2 enemyPos = transform.position;
        Vector2 targetPos = _target.transform.position;

        float distance = Vector3.Distance(transform.position, _target.transform.position);

        //For static enemy
        if (_isStaticEnemy)
        {
            if (Mathf.Abs(distance) >= _viewRange)
            {
                AttackPlayer();
                return;
            }
        }
        //For moving enemy
        base.ChasePlayer();
    }
    public override void AttackPlayer()
    {
        _weapon.up = _target.transform.position - transform.position;
        Vector2 direction = (_target.transform.position - proj_SpawnPos.position).normalized;
        GameObject GO_proj;
        if (Time.time >= _nextAttack)
        {
            switch(enum_BulletOwner)
            {
                case BULLETOWNER.GREENBAY:
                    {
                        GO_proj = _pool.GetProjectile();
                        Projectiles _proj = GO_proj.GetComponent<Projectiles>();
                        _proj.SetOwner(enum_BulletOwner);
                        _proj.SetDirection(direction);
                        _proj.SetPosition(proj_SpawnPos.position);
                        _proj.SetDamage(so_projectileStats._damage);
                        _proj.SetSpeed(so_projectileStats._speed);
                        break;
                    }
                case BULLETOWNER.WIZARD:
                    {
                        GO_proj = _pool.GetDestructibleProjectile();
                        DestructableProjectiles _proj = GO_proj.GetComponent<DestructableProjectiles>();
                        _proj.SetDestrucableProjectileType(DESTRUCTABLETYPE.REVERSAL);
                        _proj.SetOwner(enum_BulletOwner);
                        _proj.SetDirection(direction);
                        _proj.SetPosition(proj_SpawnPos.position);
                        _proj.SetDamage(so_projectileStats._damage);
                        _proj.SetSpeed(so_projectileStats._speed);
                        break;
                    }
            }
            _nextAttack = Time.time + _attackRate; // schedule next attack }
        }
    }

}
