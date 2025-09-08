using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<BaseEnemy> _EnemyList = new List<BaseEnemy> ();
    [SerializeField]internal CameraShake _camShake;
    bool _isRoomCleared;

    [SerializeField]GameObject _roomDoor;

    public void AddEnemyToList(BaseEnemy _enemy)
    {
        _EnemyList.Add (_enemy);
        _enemy.OnEnemyDied += OnEnemyDeath;
        _camShake.AddCamShakeOnDeathEvent(_enemy);
        _camShake.RemoveCamShakeOnDeathEvent(_enemy);
    }

    public void OnEnemyDeath(BaseEnemy _enemy)
    {
        if (!_EnemyList.Contains(_enemy))
            return;
        
        _enemy.gameObject.SetActive (false);
        _EnemyList.Remove(_enemy);

        if (_isRoomCleared)
            { return; }

        if(_EnemyList.Count <= 0)
        {
            _isRoomCleared = true;
            _roomDoor.SetActive(false);
        }
    }

    public GameObject GetNearestEnemyToPlayer()
    {
        GameObject _nearestEnemy = null;
        float _nearestdistance = PlayerManager.Instance._HomingDistance;
        for (int i = 0; i < _EnemyList.Count; i++)
        {
            float distance = Vector2.Distance(PlayerManager.Instance.transform.position, _EnemyList[i].gameObject.transform.position);

            if (distance < _nearestdistance)
            {
                if(_EnemyList[i].gameObject.GetComponent<BaseEnemy>().isVulnerable())
                {
                    _nearestEnemy = _EnemyList[i].gameObject;
                    _nearestdistance = distance;
                }
            }
        }
        return _nearestEnemy;
    }
    public void SetRoomManager() 
    {
        PlayerManager.Instance._roomManager = this;
    }

}
