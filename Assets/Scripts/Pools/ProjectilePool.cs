using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    [SerializeField] GameObject _projectile;
    [SerializeField] GameObject _destructibleProjectile;
    [SerializeField]
    int _amountToSpawn;

    List<GameObject> _projectilepool = new List<GameObject>();
    List<GameObject> _destructibleProjectilePool = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < _amountToSpawn; i++)
        {
            GameObject proj = Instantiate(_projectile);
            _projectilepool.Add(proj);
            proj.SetActive(false);
        }        
        for (int i = 0; i < _amountToSpawn; i++)
        {
            GameObject proj = Instantiate(_destructibleProjectile);
            _destructibleProjectilePool.Add(proj);
            proj.SetActive(false);
        }
    }

    public GameObject GetProjectile()
    {
        for (int i = 0; i < _projectilepool.Count; ++i)
        {
            if (!_projectilepool[i].gameObject.activeInHierarchy)
            {
                _projectilepool[i].gameObject.SetActive(true);
                return _projectilepool[i];

            }
        }
        return null;
    } 
    public GameObject GetDestructibleProjectile()
    {
        for (int i = 0; i < _destructibleProjectilePool.Count; ++i)
        {
            if (!_destructibleProjectilePool[i].gameObject.activeInHierarchy)
            {
                _destructibleProjectilePool[i].gameObject.SetActive(true);
                return _destructibleProjectilePool[i];

            }
        }
        return null;
    }
}
