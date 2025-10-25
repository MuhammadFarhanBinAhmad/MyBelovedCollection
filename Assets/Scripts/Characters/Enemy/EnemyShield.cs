using Unity.Cinemachine;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IResettable
{
    [SerializeField] int _baseHealth;
    int _currentHealth;
    [SerializeField] RoomManager room;

    private void OnEnable()
    {
        room = transform.parent.transform.parent.transform.parent.GetComponent<RoomManager>();
        if (room != null)
            room.RegisterResettable(this);
    }
    private void Start()
    {
        _currentHealth = _baseHealth;
    }
    public void ResetObject()
    {
        print("hit");
        _currentHealth = _baseHealth;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Projectiles>() != null)
        {
            Projectiles p = other.GetComponent<Projectiles>();
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_ShieldHit, this.transform.position);

            if (p._BulletOwner == BULLETOWNER.PLAYER)
            {
                p.SelfDestruct();
                _currentHealth--;
            }
        }

        if(_currentHealth <=0)
        {
            gameObject.SetActive(false);
        }
    }
}
