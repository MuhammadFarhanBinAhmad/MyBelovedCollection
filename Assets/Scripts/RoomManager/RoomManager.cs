using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{

    UIManager _UIManager;

    public List<BaseEnemy> _EnemyList = new List<BaseEnemy> ();
    public List<IResettable> _resettables = new List<IResettable>();
    [SerializeField]internal CameraShake _camShake;

    [SerializeField] Transform _respawnPoint;

    private void OnEnable()
    {
        if(_UIManager == null)
            _UIManager = FindAnyObjectByType<UIManager>();
    }

    public void RegisterResettable(IResettable resettable)
    {
        if (!_resettables.Contains(resettable))
            _resettables.Add(resettable);
    }

    public void AddEnemyToList(BaseEnemy _enemy)
    {
        for (int i = 0; i < _EnemyList.Count; i++)
        {
            if (_EnemyList[i] == _enemy) return;
        }

        _EnemyList.Add (_enemy);
        _enemy.OnEnemyDied += OnEnemyDeath;
        _camShake.AddCamShakeOnDeathEvent(_enemy);
        _camShake.RemoveCamShakeOnDeathEvent(_enemy);
    }

    public void OnEnemyDeath(BaseEnemy _enemy)
    {
        if (!_EnemyList.Contains(_enemy))
            return;
        
        //_enemy.gameObject.SetActive(false);
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
        PlayerManager.Instance.SetRespawnZone(_respawnPoint);
        _UIManager.MoveNextRoomFlash();


        //Activate all enemy
        for(int i=0; i < _EnemyList.Count;i++)
        {
            _EnemyList[i].gameObject.SetActive(true);
            _EnemyList[i]._playerInRoom = true;
        }
    }
    public void ExitRoom()
    {
        for (int i = 0; i < _EnemyList.Count; i++)
        {
            _EnemyList[i].ResetObject();
            _EnemyList[i].gameObject.SetActive(false);
            _EnemyList[i]._playerInRoom = false;
        }
    }

    public void RespawnPlayer() => StartCoroutine(RespawnPlayerCoroutine());

    IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(1f);
        ResetRoom();
        PlayerManager.Instance.ResetStats();
    }
    public void ResetRoom()
    {
        foreach(var enemy in _EnemyList) 
            enemy.gameObject.SetActive(true);
        foreach (var resettable in _resettables)
            resettable.ResetObject();
    }
}
