using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]SO_Character SO_character;

     internal int _BaseHealth;
     internal int _Health;

     internal int _BaseSpeed;
     internal int _Speed;

    public void OnEnable()
    {
        _BaseHealth = SO_character._BaseHealth;
        _BaseSpeed = SO_character._BaseSpeed;

        _Health = _BaseHealth;
        _Speed = _BaseSpeed;
    }
    public void TakeDamage(int dmg)
    {
        _Health -= dmg;

        if(_Health < 0 )
        {
            gameObject.SetActive(false);
        }
    }

}
