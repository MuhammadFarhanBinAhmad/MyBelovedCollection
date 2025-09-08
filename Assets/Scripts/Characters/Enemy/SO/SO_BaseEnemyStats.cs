using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyStats", menuName = "Scriptable Objects/MeleeEnemy")]
public class SO_BaseEnemyStats : ScriptableObject
{
    public float _baseSpeed;
    public int _baseHealth;
    public float _attackRate;
    public float _stopDistance;
    public float _viewrange;
    public float _waitDuration;
}
