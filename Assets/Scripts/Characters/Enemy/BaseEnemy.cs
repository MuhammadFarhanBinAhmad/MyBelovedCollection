using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour , IResettable
{

    [SerializeField]internal SO_BaseEnemyStats so_baseStats;

    public enum STATE
    {
        PATROLLING,
        ATTACKING
    }

    protected GameObject _target;
    protected Rigidbody2D _rigidbody;
    protected SpriteRenderer _spriteRenderer;

    internal STATE _state;

    [SerializeField] LayerMask _detectionMask;
    [Header("Combat Stats")]
    protected float _currentSpeed;
    protected float _stopDistance;
    protected float _viewRange;
    protected float _attackRate;
    protected float _nextAttack;
    [SerializeField]protected int _currentHealth;
    internal bool _hasInvulnerableShield;

    [Header("Patrol Stats")]
    [SerializeField] Transform[] _patrolPosition = new Transform[2];
    protected int _currentPatrolIndex = 0;
    protected float _waitTimer = 0f;
    protected float _waitDuration;
    [Header("Vulnereable state")]
    [SerializeField] float _vulnerableTime;
    [SerializeField] float _vulnerableThreshold;
    [SerializeField] float _stunDuration;
    bool _vulnerable;

    [Header("HitColour")]
    Color _originalColor;
    [SerializeField] Color _stunColor;


    [SerializeField] bool _chase_AfterDetectingOrDamaged = true;


    public event Action<BaseEnemy> OnEnemyDied;
    public event Action<BaseEnemy> OnEnemyHit;

    // --- Store initial state for reset ---
    private Vector3 _startPos;
    private Quaternion _startRot;
    private Vector3 _startScale;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        SetValue();
        _state = STATE.PATROLLING;


        // Save initial state
        _startPos = transform.position;
        _startRot = transform.rotation;
        _startScale = transform.localScale;

        FindAnyObjectByType<Player_Combo>().AddEnemyToComboCountList(this);
        FindAnyObjectByType<CameraFlash>().AddCameraFlashEvent(this);

        RoomManager room = GetComponentInParent<RoomManager>();
        if (room != null)
            room.RegisterResettable(this);

    }
    private void OnDisable()
    {
        RoomManager room = GetComponentInParent<RoomManager>();
        if (room != null)
            room.UnregisterResettable(this);
    }
    protected virtual void SetValue()
    {
        _currentSpeed = so_baseStats._baseSpeed;
        _currentHealth = so_baseStats._baseHealth;
        _stopDistance = so_baseStats._stopDistance;
        _viewRange = so_baseStats._viewrange;
        _attackRate = so_baseStats._attackRate;
        _waitDuration = so_baseStats._waitDuration;
        _hasInvulnerableShield = so_baseStats._hasInvulnerableShield;

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
    public virtual void DetectPlayer()
    {
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDir, _viewRange, _detectionMask);

        Debug.DrawRay(transform.position, facingDir * _viewRange, Color.red); // Debug line in Scene view

        if(_chase_AfterDetectingOrDamaged)
        {
            if (hit)
                _state = STATE.ATTACKING;
        }

    }
    public virtual void  ChasePlayer()
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

    public virtual void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        if (_currentHealth > 0)
        {
            // Stop all movement during stun
            _rigidbody.linearVelocity = Vector2.zero;

            // Trigger stunned state
            StartCoroutine(TempStun());

            // If enemy can chase after being hit
            _state = STATE.ATTACKING;
            OnEnemyHit?.Invoke(this);

            // Vulnerable check
            if (_currentHealth < so_baseStats._baseHealth * _vulnerableThreshold && !_vulnerable)
            {
                _vulnerable = true;
                Invoke(nameof(IsNotVulnerable), _vulnerableTime);
            }
        }
        else
        {
            OnEnemyDied?.Invoke(this);
        }
    }
    IEnumerator TempStun()
    {
        float originalSpeed = _currentSpeed;
        _currentSpeed = 0f; // temporarily stop movement
        _spriteRenderer.color = _stunColor;
        yield return new WaitForSeconds(_stunDuration);
        _spriteRenderer.color = _originalColor;
        yield return new WaitForSeconds(_stunDuration);
        _spriteRenderer.color = _stunColor;        
        yield return new WaitForSeconds(_stunDuration * 2);
        _spriteRenderer.color = _originalColor;
        _currentSpeed = originalSpeed; // restore movement
    }

    void IsNotVulnerable(){_vulnerable = false;}

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetComponent<RoomManager>() != null)
        {
            print("hit");
            RoomManager _roomManager = other.GetComponent<RoomManager>();
            _roomManager.AddEnemyToList(this);

        }
        Player_HomingCollider player_hc = other.GetComponent<Player_HomingCollider>();
        if (player_hc != null)
        {
            TakeDamage(_currentHealth);
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_EnemyHit, this.transform.position);
            PlayerManager.Instance.StopHoming();
            PlayerManager.Instance.HomingKnockBack();
        }
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }

    public bool isVulnerable() => _vulnerable;

    public void ResetObject()
    {
        CancelInvoke();
        _rigidbody.linearVelocity = Vector2.zero;

        // Reset transform
        transform.position = _startPos;
        transform.rotation = _startRot;
        transform.localScale = _startScale;

        // Reset stats
        _vulnerable = false;

        // Reset AI state
        _currentPatrolIndex = 0;
        _waitTimer = 0f;
        _state = STATE.PATROLLING;
        _target = null;

        SetValue();

        gameObject.SetActive(true); // ensure enemy is active again
    }
}
