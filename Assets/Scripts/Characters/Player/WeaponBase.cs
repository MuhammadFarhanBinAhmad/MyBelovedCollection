using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public SO_Weapon so_WeaponType;
    internal UIManager _UIManager;


    protected Camera _mainCam;
    protected ProjectilePool _projectilePool;

    public Transform _SpawnPosition;

    [Header("WeaponFireRate")]
    protected float _fireRate;
    protected float _nextTimeToFire;

    public virtual void Start()
    {
        _mainCam = Camera.main;
        _projectilePool = FindAnyObjectByType<ProjectilePool>();
        _UIManager = FindAnyObjectByType<UIManager>();
    }
    public abstract void ShootWeapon();


    /// <summary>
    /// Weapon currently is reloading
    /// </summary>
}
