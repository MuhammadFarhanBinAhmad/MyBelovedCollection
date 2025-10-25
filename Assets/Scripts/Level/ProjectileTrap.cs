using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{

    [SerializeField] SO_ProjectileStats so_projectileStats;

    ProjectilePool _pool;

    [SerializeField] Transform proj_SpawnPos;
    float _nextAttack;
    [SerializeField] float _attackRate;
    bool inView;

    private Camera _playerCam;

    private void Start()
    {
        _pool = FindAnyObjectByType<ProjectilePool>();

        _playerCam = Camera.main;
    }

    private void FixedUpdate()
    {
        if(inView = IsInCameraView())
        ShootProjectile();
    }

    public void ShootProjectile()
    {

        if (Time.time >= _nextAttack)
        {
            GameObject GO_proj = _pool.GetProjectile();
            Projectiles _proj = GO_proj.GetComponent<Projectiles>();


            _proj.SetOwner(BULLETOWNER.OBSTACLE);
            _proj.SetDirection(transform.right);
            _proj.SetPosition(proj_SpawnPos.position);
            _proj.SetDamage(so_projectileStats._damage);
            _proj.SetSpeed(so_projectileStats._speed);

            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_ProjectileTrap,transform.position);

            _nextAttack = Time.time + _attackRate; // schedule next attack }
        }
    }
    private bool IsInCameraView()
    {
        if (_playerCam == null) return false;

        Vector3 viewPos = _playerCam.WorldToViewportPoint(transform.position);

        return (viewPos.z > 0 &&
                viewPos.x > 0 && viewPos.x < 1 &&
                viewPos.y > 0 && viewPos.y < 1);
    }
}
