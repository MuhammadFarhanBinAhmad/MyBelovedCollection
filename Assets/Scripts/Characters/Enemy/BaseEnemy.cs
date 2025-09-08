using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{

    [SerializeField]internal SO_BaseEnemyStats so_baseStats;

    public enum STATE
    {
        PATROLLING,
        ATTACKING
    }

    [SerializeField]protected GameObject _target;
    [SerializeField] protected Rigidbody2D _rigidbody;

    STATE _state;


    [SerializeField] LayerMask _detectionMask;
    [Header("Combat Stats")]
    protected float _currentSpeed;
    protected float _stopDistance;
    protected float _viewRange;
    protected float _attackRate;
    protected float _nextAttack;
    [SerializeField]protected int _currentHealth;

    [Header("Patrol Stats")]
    [SerializeField] Transform[] _patrolPosition = new Transform[2];
    protected int _currentPatrolIndex = 0;
    protected float _waitTimer = 0f;
    protected float _waitDuration;
    [Header("Vulnereable state")]
    [SerializeField] float _vulnerableTime;
    [SerializeField] float _vulnerableThreshold;
    bool _vulnerable;


    public event Action<BaseEnemy> OnEnemyDied;
    public event Action<BaseEnemy> OnEnemyHit;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        SetValue();
        _state = STATE.PATROLLING;
        FindAnyObjectByType<Player_Combo>().AddEnemyToComboCountList(this);
        FindAnyObjectByType<CameraFlash>().AddCameraFlashEvent(this);

    }

    protected virtual void SetValue()
    {
        _currentSpeed = so_baseStats._baseSpeed;
        _currentHealth = so_baseStats._baseHealth;
        _stopDistance = so_baseStats._stopDistance;
        _viewRange = so_baseStats._viewrange;
        _attackRate = so_baseStats._attackRate;
        _waitDuration = so_baseStats._waitDuration;
    }
    private void Update()
    {

        switch (_state)
        {
            case STATE.PATROLLING:
                {
                    Patrolling();
                    DetectPlayer();
                    break;
                }
            case STATE.ATTACKING:
                {
                    ChasePlayer();
                    break;
                }
        }
    }

    void Patrolling()
    {
        if (_patrolPosition.Length < 2) return; // Needs 2 points

        Transform targetPoint = _patrolPosition[_currentPatrolIndex];
        float distance = Vector2.Distance(transform.position, targetPoint.position);

        if (distance <= 0.1f)
        {
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y); // stop moving

            // Reached patrol point -> idle
            if (_waitTimer <= 0f)
            {
                _waitTimer = _waitDuration; // start wait
            }
            else
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    // Switch to next patrol point
                    _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPosition.Length;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1;
                    transform.localScale = localScale;
                }

            }
        }
        else
        {
            Vector2 direction = (targetPoint.position - transform.position).normalized;
            _rigidbody.linearVelocity = new Vector2(direction.x * _currentSpeed, _rigidbody.linearVelocity.y);
        }
    }
    void DetectPlayer()
    {
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDir, _viewRange, _detectionMask);

        Debug.DrawRay(transform.position, facingDir * _viewRange, Color.red); // Debug line in Scene view
        if (hit)
            _state = STATE.ATTACKING;
    }
    public void ChasePlayer()
    {
        if (_target == null)
            _target = PlayerManager.Instance.gameObject;

        Vector2 enemyPos = transform.position;
        Vector2 targetPos = _target.transform.position;

        float distance = Vector3.Distance(transform.position, _target.transform.position);

        if (Mathf.Abs(distance) >= _stopDistance)
        {
            Vector2 direction = (targetPos - enemyPos).normalized;

            // move only on X axis
            _rigidbody.linearVelocity = new Vector2(direction.x * _currentSpeed, _rigidbody.linearVelocity.y);
        }
        else
        {
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y); // stop when attacking
            AttackPlayer();
        }
    }

    public virtual void AttackPlayer(){}

    public void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        if (_currentHealth > 0)
        {
            _state = STATE.ATTACKING;
            OnEnemyHit?.Invoke(this);
            if(_currentHealth < so_baseStats._baseHealth * _vulnerableThreshold
                && !_vulnerable)
            {
                _vulnerable = true;
                Invoke("IsNotVulnerable",_vulnerableTime);
            }
        }
        else
        {
            OnEnemyDied?.Invoke(this);
        }
    }
    void IsNotVulnerable(){_vulnerable = false;}

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetComponent<RoomManager>() != null)
        {
            RoomManager _roomManager = other.GetComponent<RoomManager>();
            _roomManager.AddEnemyToList(this);

        }
        if (other.GetComponent<Projectiles>() != null)
        {
            Projectiles temp_Proj = other.GetComponent<Projectiles>();
            if(temp_Proj._BulletOwner == BULLETOWNER.PLAYER)
            {
                TakeDamage(temp_Proj.GetDamage());
                temp_Proj.SelfDestruct();
            }

            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_EnemyHit, this.transform.position);
        }
        if (other.GetComponent<Player_HomingCollider>() != null)
        {
            TakeDamage(_currentHealth);
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_EnemyHit, this.transform.position);
            PlayerManager.Instance.StopHoming();
            PlayerManager.Instance.HomingKnockBack();
        }
    }

    public bool isVulnerable() => _vulnerable;
}
