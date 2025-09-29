using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public List<BaseEnemy> _EnemyList = new List<BaseEnemy> ();
    [SerializeField]internal CameraShake _camShake;

    [SerializeField] Transform _respawnPoint;

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
    }

    public void RespawnPlayer() => StartCoroutine(RespawnPlayerCoroutine());

    IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(1);
        PlayerManager.Instance.ResetStats();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
