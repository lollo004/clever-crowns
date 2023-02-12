using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    //stats and description
    [SerializeField] public int Health = 0;
    [SerializeField] public int Damage = 0;
    [SerializeField] public int Speed = 0;
    [SerializeField] public int Weight = 0;
    [SerializeField] public int Cost = 0;
    [SerializeField] public string Name = "";
    [SerializeField] public string Effect = "";
    [SerializeField] public string Description = "";
    [SerializeField] public Sprite Image;

    //management and game login
    public string team = ""; // player, enemy, cpu
    public string position = ""; // hand, attack, defense
    public string _currentPos = ""; // hand, attack, defense
    public int placement = -1;
    public bool isAttacking = false;
    public bool isDefending = false;
    public bool isMoving = false;

    //variable to check if you selected an item with the attack to avoid to bug the attackers queue
    public bool isTargetLocated = false;

    //game manager
    public GameManager gameManager;
    //graphic pointer
    public GameObject pointer;

    //watch a card
    Collider2D OverlappingCollider;

    //drag and drop
    private Vector2 difference = Vector2.zero;
    private Vector2 start_position = Vector2.zero;
    public GameObject[] valid_defense_positions = new GameObject[4];
    public GameObject[] valid_attack_positions = new GameObject[5];
    private Vector2 newPos = Vector2.zero;
    private GameObject card_place;

    void Start()
    {
        //find game manager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        valid_defense_positions = GameObject.FindGameObjectsWithTag("Defense");
        valid_attack_positions = GameObject.FindGameObjectsWithTag("Attack");
    }
    
    // Update is called once per frame
    private void OnMouseDown() // mouse pressed
    {
        if (gameManager.turn == "player" && team == "player" && gameManager.phase == "attack")
        {
            //drag and drop if the card is in your hand and you have enough lymph
            difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
            start_position = transform.position;

            //attack with you card if you can
            if (position == "attack" && gameManager.phase == "attack")
            {
                //if the card is already attacking then stop it, otherways if it's not then show the pointer
                if (isAttacking)
                {
                    isAttacking = false;
                    if (isTargetLocated)
                    {
                        isTargetLocated = false;
                        gameManager.PlayerNumberOfAttackers--;
                        gameManager.PlayerAttacckers.Remove(placement);
                    }
                }
                else
                {
                    if (gameManager.PlayerNumberOfAttackers < gameManager.Stress)
                    {
                        isAttacking = true;
                        pointer = (GameObject)Instantiate(Resources.Load("Pointer"));
                    }
                }
            }
        }
    }
    private void OnMouseDrag() // mouse moved while pressed
    {
        if (gameManager.turn == "player" && team == "player" && gameManager.phase == "attack")
        {
            //drag the card or the pointer
            isMoving = true;
            if (position == "hand" && gameManager.phase == "attack")
            {
                transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
            }
            if (position == "attack" && gameManager.phase == "attack" && isAttacking)
            {
                pointer.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
            }
        }
    }

    private void OnMouseUp() // mouse relesed
    {
        if (gameManager.turn == "player" && team == "player" && gameManager.phase == "attack")
        {
            //drop the cardif it's in your hand and you have enough lymph
            if (position == "hand" && gameManager.Lymph >= Cost && newPos != Vector2.zero)
            {
                gameManager.Lymph -= Cost;
                transform.position = newPos;
                position = _currentPos;
                card_place.SetActive(false); //ricordarsi di attivarlo una volta morta la carta
            }
            else
            {
                transform.position = start_position;
            }

            //delete the pointer and select the cart to attack
            if (isAttacking)
            {
                GameObject.Destroy(pointer);
                //check if you select the enemy to attack
                if (gameManager.target.GetComponent<Card>().team != team)
                {
                    gameManager.PlayerNumberOfAttackers++;
                    gameManager.PlayerAttacckers.Add(placement);
                    isTargetLocated = true;
                }
            }
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        card_place = collision.gameObject;
        try
        {
            for (int i = 0; i < 5; i++)
            {
                if (collision.gameObject == valid_attack_positions[i])
                {
                    newPos = collision.gameObject.transform.position;
                    _currentPos = "attack";
                    placement = i + 5;
                    break;
                }
                if (collision.gameObject == valid_defense_positions[i])
                {
                    newPos = collision.gameObject.transform.position;
                    _currentPos = "defense";
                    placement = i+1;
                    break;
                }
            }
        }
        catch { } // avoid the exception throwed on the 5th element of defense
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (position == "hand" && !collision.gameObject.GetComponent<Card>())
        {
            newPos = Vector2.zero;
            placement = -1;
        }
    }
    ///* card growing when overlapping
    private void OnMouseEnter()
    {
        OverlappingCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (position == "hand" && OverlappingCollider.gameObject == gameObject)
        {
            transform.localScale = new Vector2(2, 3);
            transform.SetPositionAndRotation(new Vector2(transform.position.x, transform.position.y + 1.5f), transform.rotation);
        }
    }

    private void OnMouseOver()
    {
        if (position == "hand" && OverlappingCollider.gameObject == gameObject)
        {
            if (isMoving)
            {   
                transform.localScale = new Vector2(1, 1.5f);
                transform.SetPositionAndRotation((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.rotation);
            }
        }
    }

    private void OnMouseExit()
    {
        if (position == "hand")
        {
            transform.SetPositionAndRotation(new Vector2(transform.position.x, transform.position.y - 1.5f), transform.rotation);
            transform.localScale = new Vector2(1, 1.5f);
        }
    }
    //*/
}
