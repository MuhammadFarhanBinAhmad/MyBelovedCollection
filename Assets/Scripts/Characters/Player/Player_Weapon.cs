using UnityEngine;

public class Player_Weapon : WeaponBase
{
    [SerializeField] float cam_shakeValue;

    private void OnEnable()
    {
        _fireRate = so_WeaponType.weapon_fireRate;
    }

    public override void Start() 
    {
        base.Start();
        _UIManager.UpdatePlayerUIObserver();
    }

    void Update()
    {
        AimAtMouse();
        ShootWeapon();
    }

    public override void ShootWeapon() 
    {

        if (Input.GetMouseButtonDown(0) && 
            _nextTimeToFire <= 0 && 
            !PlayerManager.Instance.GetIsDashing())
        {
            SpawnProjectile();
            _nextTimeToFire = _fireRate;

        }
        else
        {
            _nextTimeToFire -= Time.deltaTime;
        }
    }
    void SpawnProjectile()
    {
        Vector3 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - _SpawnPosition.position).normalized;

        GameObject go_proj = _projectilePool.GetProjectile();
        Projectiles proj = go_proj.GetComponent<Projectiles>();
        proj.SetOwner(BULLETOWNER.PLAYER);
        proj.SetDirection(direction);
        proj.SetPosition(_SpawnPosition.position);
        proj.SetDamage(so_WeaponType.ammo_damage);
        proj.SetSpeed(so_WeaponType.ammo_speed);

        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_GunShot, this.transform.position);
    }
    void AimAtMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Get direction from weapon to mouse
        Vector3 direction = mousePos - transform.position;

        // Calculate angle and apply rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle-90);
    }
}
