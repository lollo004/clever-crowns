using UnityEngine;

public class Pointer : MonoBehaviour
{
    //game manager
    public GameObject card;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        card.GetComponent<Card>().current_target = collision.gameObject;
        card.GetComponent<Card>().isAttacking = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        card.GetComponent<Card>().current_target = null;
    }
}
