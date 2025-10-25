using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]SO_Character SO_character;

     internal int _BaseHealth;
     internal int _Health;

     internal int _BaseSpeed;
     internal int _Speed;

    internal bool _Moving;
    internal bool _IsMoving;
    internal bool _wasMoving;

    public void OnEnable()
    {
        _BaseHealth = SO_character._BaseHealth;
        _BaseSpeed = SO_character._BaseSpeed;

        _Health = _BaseHealth;
        _Speed = _BaseSpeed;
    }


}
