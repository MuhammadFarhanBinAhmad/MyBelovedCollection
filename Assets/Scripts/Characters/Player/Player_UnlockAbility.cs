using UnityEngine;

public class Player_UnlockAbility : MonoBehaviour
{
    [SerializeField]PlayerAbility _ability;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerManager>())
        {
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_NewAbility, this.transform.position);
            PlayerManager.Instance.UnlockAbility(_ability);
            Destroy(this.gameObject);
            //need add event to showcase what they got and some dialouge
        }
    }
}
