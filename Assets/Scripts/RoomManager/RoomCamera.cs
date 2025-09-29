using Unity.Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{

    [SerializeField]  GameObject _virtualcamera;
    [SerializeField] CameraShake _CameraShake;
    [SerializeField] CinemachineCamera _cineMachineCamera;

    private void OnEnable()
    {
        _CameraShake = FindAnyObjectByType<CameraShake>();
        _cineMachineCamera = _virtualcamera.GetComponent<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerManager>() != null)
        {
            _virtualcamera.SetActive(true);
            _cineMachineCamera.Follow = other.transform;
            _cineMachineCamera.LookAt = other.transform;

            GetComponent<RoomManager>().SetRoomManager();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<PlayerManager>().GetIsDead())
        {
            _virtualcamera.SetActive(false);
        }
    }
}

