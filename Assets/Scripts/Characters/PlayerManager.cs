using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using FMOD.Studio;
using static UnityEngine.ParticleSystem;
public class PlayerManager : Character
{

    [SerializeField]internal RoomManager _roomManager;
    [SerializeField] BoxCollider2D _homingCollider;

    [Header("Jump and Ground Check Settings")]
    public float _jumpForce;
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


    [Header("Audio")]

    EventInstance sfx_PlayerFootStep;


    public bool GetIsDashing() { return _isDashing; }
    public void SetIsDashinag(bool dashing) { _isDashing = dashing; }

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null)
        {
            print("Fuck");
        }
        Instance = this;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        sfx_PlayerFootStep = AudioManager.Instance.CreateEventInstance(FmodEvent.Instance.sfx_PlayerFootStep);
        _homingCollider.gameObject.SetActive(false);
    }

    void Update()
    {
        Movement();
        HandleRotation();
        //HandleWallJump();
        StartDash();
        StartHomingAttack();
        UpdateSound();
    }


    void Movement()
    {
        if (!_isDashing)
        {
            float horizontal = Input.GetAxis("Horizontal");
            if (_isGrounded)
            {
                // Movement
                _rigidbody.linearVelocity = new Vector2(horizontal * _Speed, _rigidbody.linearVelocity.y);
            }
            else if(!_isGrounded)
            {
                // Air Movement
                _rigidbody.linearVelocity = new Vector2(horizontal * (_Speed/2), _rigidbody.linearVelocity.y);
            }
        }

        // Ground check
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (_isGrounded)
            {
                // Normal Jump
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
            }
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

        // If player presses Jump while touching a wall (but not grounded)
        if (Input.GetButtonDown("Jump") && !_isGrounded)
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

        // Debug rays
        Debug.DrawRay(origin, Vector2.left * _wallCheckDistance, Color.red);
        Debug.DrawRay(origin, Vector2.right * _wallCheckDistance, Color.green);
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
        if(_rigidbody.linearVelocityX != 0 && _isGrounded)
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
}
