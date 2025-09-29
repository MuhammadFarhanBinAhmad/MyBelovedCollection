using System.Collections;
using UnityEngine;
using FMOD.Studio;
using System;
using UnityEngine.Events;
using System.Collections.Generic;


public enum PlayerAbility
{
    JUMP,
    GUN,
    DASH,
    HOMING,
    WALLJUMP
}

public class PlayerManager : Character
{

    private Dictionary<PlayerAbility, bool> _abilities = new Dictionary<PlayerAbility, bool>();

    [SerializeField]internal RoomManager _roomManager;
    UIManager _UIManager;

    [Header("Jump and Ground Check Settings")]
    public float _jumpForce;
    [SerializeField] private float _fallGravityMultiplier = 2.5f;   // Gravity when falling
    [SerializeField] private float _lowJumpGravityMultiplier = 2f;  // Gravity when jump is cut short
    [SerializeField] private float _coyoteTime = 0.1f;              // Time after leaving ground you can still jump
    [SerializeField] private float _jumpBufferTime = 0.1f;          // Buffer for early jump input
    [SerializeField] private float _airControlMultiplier = 0.8f;    // Control while in air
    [SerializeField] private float _acceleration = 10f;             // Acceleration rate
    [SerializeField] private float _deceleration = 15f;             // Deceleration when no input

    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    public Transform _groundCheck;
    public float _groundCheckRadius;
    public LayerMask _groundLayer;

    private Rigidbody2D _rigidbody;
    [SerializeField]
    private bool _isGrounded;

    [Header("Rotation Settings")]
    public float _spinSpeed; // degrees per second
    public float _grounddistance;
    public float base_timegroundcheck;
    public float current_timegroundcheck;
    int _spinDirection = 1;

    [Header("Dash Settings")]
    public TrailRenderer _trail;
    public float _dashForce;
    public float _dashDuration;
    public float _dashCooldown;
    float _dashCurrentCooldownTime;
    private bool _isDashing = false;

    [Header("Homing Settings")]
    GameObject _homingTarget;
    [SerializeField] BoxCollider2D _homingCollider;
    [SerializeField]internal float _HomingDistance;
    [SerializeField]internal float _homingKnockBack;
    bool _isHoming;

    [Header("Wall Jump Settings")]
    public float _wallJumpForce;
    public float _wallJumpHorizontalForce;
    float _wallJumpDirectionForce;
    public float _wallCheckDistance;
    public float _wallCooldown;
    private bool _isWallJumping = false;

    [Header("Knockback Settings")]
    bool _isHit;
    public float _knockbackForce;
    public float _knockbackDuration = 0.5f;
    private Vector2 _knockbackDirection;

    [Header("DamageFeedback")]
    public GameObject _playerWeapon;
    BoxCollider2D _playerCollider;
    [Header("DamageFeedback")]
    [SerializeField] GameObject _playerSprite;
    Transform _respawnZone;
    bool _isDead;

    public event UnityAction<PlayerManager> OnPlayerDied;

    [Header("Audio")]
    EventInstance sfx_PlayerFootStep;


    public bool GetIsDead() => _isDead;
    public bool GetIsDashing() { return _isDashing; }
    public void SetIsDashinag(bool dashing) { _isDashing = dashing; }
    public void SetRespawnZone(Transform _pos) => _respawnZone = _pos;
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null)
        {
            print("Fuck");
        }
        Instance = this;

        foreach (PlayerAbility ability in Enum.GetValues(typeof(PlayerAbility)))
        {
            _abilities[ability] = false; // default locked
        }
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        sfx_PlayerFootStep = AudioManager.Instance.CreateEventInstance(FmodEvent.Instance.sfx_PlayerFootStep);
        FindAnyObjectByType<CameraFlash>().AddCameraFlashEvent(this);
        _UIManager = FindAnyObjectByType<UIManager>();
        _homingCollider.gameObject.SetActive(false);
        _playerCollider = GetComponent<BoxCollider2D>();


    }

    void Update()
    {

        if(!_isDead)
        {
            Movement();
            HandleRotation();
        }
        if (HasAbility(PlayerAbility.WALLJUMP))
        HandleWallJump();

        if(HasAbility(PlayerAbility.DASH))
        StartDash();

        if(HasAbility(PlayerAbility.HOMING))
        StartHomingAttack();


        UpdateSound();
    }


    void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (!_isDashing)
        {
            float targetSpeed = horizontal * _Speed;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _deceleration;

            if (!_isGrounded) accelRate *= _airControlMultiplier;

            // Smooth acceleration/deceleration
            _rigidbody.linearVelocity = new Vector2(
                Mathf.Lerp(_rigidbody.linearVelocity.x, targetSpeed, accelRate * Time.deltaTime),
                _rigidbody.linearVelocity.y
            );
        }

        // ------------------------------
        // Ground Check
        // ------------------------------
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

        if (_isGrounded)
            _coyoteTimeCounter = _coyoteTime; // reset coyote
        else
            _coyoteTimeCounter -= Time.deltaTime;


        // Jump


        if (HasAbility(PlayerAbility.JUMP))
        {
            if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
                _jumpBufferCounter = 0f; // consume buffered jump
            }

            // Variable jump height
            if (_rigidbody.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                _rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (_lowJumpGravityMultiplier - 1) * Time.deltaTime;
            }
            else if (_rigidbody.linearVelocity.y < 0)
            {
                _rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallGravityMultiplier - 1) * Time.deltaTime;
            }
            if (Input.GetButtonDown("Jump"))
                _jumpBufferCounter = _jumpBufferTime;
            else
                _jumpBufferCounter -= Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        if (IsNearGround())
        {
            // Smoothly rotate back to upright (0,0,0)
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            // Spin in air
            transform.Rotate(Vector3.forward * _spinSpeed * Time.deltaTime);
        }
        if (_isGrounded)
        {
            current_timegroundcheck = base_timegroundcheck;
        }
    }
    bool IsNearGround()
    {
        if(!_isGrounded && current_timegroundcheck > 0)
        {
            current_timegroundcheck -= Time.deltaTime;
            return false;
        }
        else
        {
            Vector2 position = transform.position;

            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, _grounddistance, _groundLayer);
            if (hit.collider != null)
            {
                return true;
            }
        }

        return false;
    }
    #region HomingAttack
    public void StartHomingAttack()
    {

        if (_isHoming)
        {
            CurrentlyHoming(_homingTarget.transform);
            return;
        }

        if (Input.GetButtonDown("Jump") && !_isGrounded)
        {
            _homingTarget = _roomManager.GetNearestEnemyToPlayer();

            if (_homingTarget == null)
                return;

            if (Vector2.Distance(transform.position, _homingTarget.transform.position) <= _HomingDistance)
            {
                _isHoming = true;
                _dashCurrentCooldownTime = _dashCooldown;
                _rigidbody.linearVelocity = Vector2.zero;
                AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_PlayerDashing, transform.position);
                StartCoroutine(Homing());
            }
        }
    }
    IEnumerator Homing()
    {
        _homingCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(_dashDuration);
        StopHoming();

    }

    void CurrentlyHoming(Transform enemyPos)
    {
        if (enemyPos == null) return;
        Vector2 dir = (enemyPos.position - transform.position).normalized;
        _rigidbody.linearVelocity = dir * _dashForce;
    }

    public void StopHoming()
    {
        CancelInvoke("CurrentlyHoming");
        _isHoming = false;
        _homingCollider.gameObject.SetActive(false);
        _homingTarget = null;
    }

    public void HomingKnockBack()
    {
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.linearVelocity = Vector2.up * _homingKnockBack;
    }
    #endregion


    #region Dashing
    void StartDash()
    {
        if(_isHoming)
            return;

        if (_isDashing)
        {
            CurrentlyDashing();
            return;
        }


        if (_dashCurrentCooldownTime > 0)
        {
            _dashCurrentCooldownTime -= Time.deltaTime;
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && _dashCurrentCooldownTime <= 0)
        {
            _isDashing = true;
            _dashCurrentCooldownTime = _dashCooldown;
            _rigidbody.linearVelocity = Vector2.zero;
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_PlayerDashing,transform.position);
            _trail.gameObject.SetActive(true);
            StartCoroutine(Dashing());
        }
    }
    IEnumerator Dashing()
    {
        yield return new WaitForSeconds(_dashDuration);
        _rigidbody.linearVelocity = Vector2.zero;
        _isDashing = false;
        _trail.Clear();
        _trail.gameObject.SetActive(false);
    }

    void CurrentlyDashing()
    {
        float dir = Input.GetAxis("Horizontal");
        _rigidbody.linearVelocity = new Vector2(dir * _dashForce, _rigidbody.linearVelocity.y);
    }
    #endregion
    #region WallJump
    void HandleWallJump()
    {
        if (_isWallJumping)
        {
            CurrentlyWallJumping();
            return;
        }


        Vector2 origin = transform.position;

        // Cast left and right
        RaycastHit2D leftHit = Physics2D.Raycast(origin, Vector2.left, _wallCheckDistance);
        RaycastHit2D rightHit = Physics2D.Raycast(origin, Vector2.right, _wallCheckDistance);

        if (leftHit.collider != null && leftHit.collider.CompareTag("Wall") ||
            rightHit.collider != null && rightHit.collider.CompareTag("Wall"))

        {
            if (Input.GetButtonDown("Jump"))
            {
                if (leftHit.collider != null && leftHit.collider.CompareTag("Wall"))
                {
                    // Jump away from LEFT wall (push right)
                    _wallJumpDirectionForce = _wallJumpHorizontalForce;
                }
                else if (rightHit.collider != null && rightHit.collider.CompareTag("Wall"))
                {
                    // Jump away from RIGHT wall (push left)
                    _wallJumpDirectionForce = -_wallJumpHorizontalForce;
                }
                _isWallJumping = true;
                _rigidbody.linearVelocity = Vector2.zero;
                AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_PlayerDashing, transform.position);
                StartCoroutine(WallJumping());
            }
        }

    }
    IEnumerator WallJumping()
    {
        yield return new WaitForSeconds(_wallCooldown);
        _isWallJumping = false;
    }

    void CurrentlyWallJumping()
    {
        _rigidbody.linearVelocity = new Vector2(_wallJumpDirectionForce, _wallJumpForce);
    }
    #endregion

    void UpdateSound()
    {
        if(Mathf.Abs(_rigidbody.linearVelocityX) >0.1 && _isGrounded)
        {
            PLAYBACK_STATE playbackstate;
            sfx_PlayerFootStep.getPlaybackState(out playbackstate);

            if (playbackstate.Equals(PLAYBACK_STATE.STOPPED))
            {
                sfx_PlayerFootStep.start();
            }
        }
        else
        {
            sfx_PlayerFootStep.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }


    #region Taking Damage
    public void TakeDamage()
    {

        OnPlayerDied?.Invoke(this);
        HidePlayerSprite();
        _rigidbody.linearVelocity = Vector2.zero;
        sfx_PlayerFootStep.stop(STOP_MODE.ALLOWFADEOUT);
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_PlayerDeath,transform.position);
        _isDead = true;
        _roomManager.RespawnPlayer();
    }

    void HidePlayerSprite()
    {
        _playerSprite.SetActive(false);
        _rigidbody.simulated = false;
        _playerCollider.enabled = false;
    }

    public void ResetStats()
    {
        _playerCollider.enabled = true;
        _rigidbody.simulated = true;
        transform.position = _respawnZone.transform.position;
        _playerSprite.SetActive(true);
        _isDead = false;
        _UIManager.UpdatePlayerUIObserver();
    }

    #endregion


    #region UnlockAbility
    // Check if unlocked
    public bool HasAbility(PlayerAbility ability) => _abilities[ability];

    // Unlock
    public void UnlockAbility(PlayerAbility ability)
    {
        if(ability == PlayerAbility.GUN)
        {
            _playerWeapon.SetActive(true);
        }
        _abilities[ability] = true;
        Debug.Log($"Unlocked ability: {ability}");
    }
    #endregion
}
