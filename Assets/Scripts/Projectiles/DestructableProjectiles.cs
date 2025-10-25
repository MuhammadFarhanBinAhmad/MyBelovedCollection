using System;
using UnityEngine;

public enum DESTRUCTABLETYPE
{
    DESTROYABLE,
    REVERSAL
}
public class DestructableProjectiles : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    [SerializeField] TrailRenderer _enemyTrailRenderer, _playerTrailRenderer;
    [SerializeField] GameObject vfx_hitEffect;

    Vector2 _direction;

    [SerializeField] float time_tillDisable;
    float time_currentTillDisable;

    [SerializeField] internal BULLETOWNER _BulletOwner;
    int _dmg, _speed;

    bool _isReflected;

    DESTRUCTABLETYPE _destrutableType;

    SpriteRenderer _spriteRenderer;
    Color _startingColor;

    public event Action<DestructableProjectiles> OnProjectileHit;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startingColor = _spriteRenderer.color;

        FindAnyObjectByType<CameraFlash>().AddCameraFlashEvent(this) ;

    }
    private void OnEnable()
    {
        time_currentTillDisable = time_tillDisable;

    }
    public void SetDamage(int dmg) { _dmg = dmg; }
    public int GetDamage() { return _dmg; }

    public void SetSpeed(int speed) { _speed = speed; }
    public int GetSpeed() { return _speed; }

    public void SetDirection(Vector2 dir)
    {
        _direction = dir;
        float spreadAngle = UnityEngine.Random.Range(-2f, 2f); // tweak range for accuracy
        float rad = spreadAngle * Mathf.Deg2Rad;

        // Rotate the direction vector by spread angle in 2D
        _direction = new Vector2(
            _direction.x * Mathf.Cos(rad) - _direction.y * Mathf.Sin(rad),
            _direction.x * Mathf.Sin(rad) + _direction.y * Mathf.Cos(rad)
        );
    }
    public void SetPosition(Vector3 pos) { transform.position = pos; }

    public void SetOwner(BULLETOWNER owner) { _BulletOwner = owner; }

    public BULLETOWNER GetOwner() { return _BulletOwner; }
    public void SetDestrucableProjectileType(DESTRUCTABLETYPE DT) => _destrutableType = DT;
    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _direction * _speed;
        if (time_currentTillDisable >= 0)
            time_currentTillDisable -= Time.deltaTime;
        else
            SelfDestruct();
    }


    internal void SelfDestruct()
    {
        _enemyTrailRenderer.Clear();
        _playerTrailRenderer.Clear();
        _isReflected = false;
        _spriteRenderer.color = _startingColor;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == this)
            return;

        switch (_BulletOwner)
        {
            case BULLETOWNER.WIZARD:
            case BULLETOWNER.HARD_CORE_HENRY:
                {
                    if (other.GetComponent<PlayerManager>() != null)
                    {
                        PlayerManager _player = other.GetComponent<PlayerManager>();
                        _player.TakeDamage();
                        SelfDestruct();
                    }
                    break;
                }
        }
        if (other.GetComponent<Projectiles>() != null)
        {
            Projectiles tmp = other.GetComponent<Projectiles>();

            if (tmp._BulletOwner == BULLETOWNER.PLAYER)
            {
                switch (_destrutableType)
                {
                    case DESTRUCTABLETYPE.DESTROYABLE:
                        {
                            tmp.SelfDestruct();
                            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_EnemyHit, this.transform.position);
                            break;
                        }
                    case DESTRUCTABLETYPE.REVERSAL:
                        {
                            if(!_isReflected)
                            {
                                tmp.SelfDestruct();
                                _isReflected = true;
                                _direction *= -1;
                                time_currentTillDisable = time_tillDisable;
                                _spriteRenderer.color = new Color(1f - _startingColor.r, 1f - _startingColor.g, 1f - _startingColor.b, _startingColor.a);
                                _playerTrailRenderer.gameObject.SetActive(true);
                                _enemyTrailRenderer.gameObject.SetActive(false);
                                AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_DeflectProjectile, this.transform.position);
                            }
                            break;
                        }
                }
                OnProjectileHit?.Invoke(this);

            }
        }

        if(_isReflected)
        {
            if (other.GetComponent<BaseEnemy>() != null)
            {
                BaseEnemy _re = other.GetComponent<BaseEnemy>();
                AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_EnemyHit, this.transform.position);
                _re.TakeDamage(_dmg * 100, this.transform);
                SelfDestruct();

            }
        }


        if (other.tag == "Ground")
        {
            Vector2 hitDirection = (other.transform.position - transform.position).normalized;
            Vector2 spawnPos = (Vector2)transform.position + (-hitDirection * 0.3f); // Offset backwards a bit

            if (vfx_hitEffect != null)
            {
                GameObject vfx = Instantiate(vfx_hitEffect, spawnPos, Quaternion.identity);
                // Optional: make it face away from bullet hit direction
                vfx.transform.right = hitDirection;
            }

            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_GeneralProjectileHit, this.transform.position);
            SelfDestruct();
        }
    }
}
