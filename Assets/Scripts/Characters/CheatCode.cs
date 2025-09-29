using UnityEngine;

public class CheatCode : MonoBehaviour
{
    PlayerManager _playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _playerManager.UnlockAbility(PlayerAbility.JUMP);
            print("Jump unlock");
        }    
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _playerManager.UnlockAbility(PlayerAbility.GUN);
            print("Gun unlock");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _playerManager.UnlockAbility(PlayerAbility.DASH);
            print("Dash unlock");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerManager.UnlockAbility(PlayerAbility.WALLJUMP);
            print("WallJump unlock");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _playerManager.UnlockAbility(PlayerAbility.HOMING);
            print("Homing unlock");
        }
    }
}
