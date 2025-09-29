using System;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public enum BULLETOWNER
{
    PLAYER,
    ENEMY
}

public class Projectiles : MonoBehaviour
{

    public event Action<Projectiles> OnEnemyHit;
    public event Action<Projectiles> OnEnemyDied;

    Rigidbody2D _rigidbody;
    [SerializeField]TrailRenderer _trailRenderer;

    Vector2 _direction;

    [SerializeField] float time_tillDisable;
    float time_currentTillDisable;

    internal BULLETOWNER _BulletOwner;
    int _dmg, _speed;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

    }
    private void OnEnable()
    {
        time_currentTillDisable = time_tillDisable;
        // Add inaccuracy (spread) for 2D bullets

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
        _trailRenderer.Clear();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (_BulletOwner)
        {
            case BULLETOWNER.ENEMY:
                {
                    if (other.GetComponent<PlayerManager>() != null)
                    {
                        PlayerManager _player = other.GetComponent<PlayerManager>();
                        _player.TakeDamage();
                        SelfDestruct();
                    }
                    break;
                }
            case BULLETOWNER.PLAYER:
                {
                    if (other.GetComponent<BaseEnemy>() != null)
                    {
                        BaseEnemy _be = other.GetComponent<BaseEnemy>();
                        _be.TakeDamage(_dmg);
                        SelfDestruct();

                        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_EnemyHit, this.transform.position);
                    }
                    break;
                }
        }


        if (other.tag == "Ground")
        {
            SelfDestruct();
        }
    }
}
