using UnityEngine;

public class TrapDoor : MonoBehaviour , IInteractables
{

    [SerializeField] GameObject _trapDoor;

    public void ActivateObject()
    {
        if(_trapDoor.activeSelf)
        _trapDoor.SetActive(false);
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_TrapDoor, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerManager>() != null)
            ActivateObject();

    }
}
