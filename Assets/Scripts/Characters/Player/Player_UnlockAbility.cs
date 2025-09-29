using UnityEngine;

public class Player_UnlockAbility : MonoBehaviour
{
    [SerializeField]PlayerAbility _ability;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerManager>())
        {
            PlayerManager.Instance.UnlockAbility(_ability);
            Destroy(this.gameObject);
            //need add event to showcase what they got and some dialouge
        }
    }
}
