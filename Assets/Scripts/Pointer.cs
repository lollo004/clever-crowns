using UnityEngine;

public class Pointer : MonoBehaviour
{
    //game manager
    public GameObject card;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        card.GetComponent<Card>().current_target = collision.gameObject;
        card.GetComponent<Card>().isAttacking = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        card.GetComponent<Card>().current_target = null;
    }
}
