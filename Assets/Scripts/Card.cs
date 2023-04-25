using UnityEngine;

public class Card : MonoBehaviour
{
    //stats and description
    [SerializeField] public int Cost = 0; //cost of the card
    [SerializeField] public string Type = ""; //type of the card (troop, magic, building)
    [SerializeField] public string Restriction = ""; //position in the field (attack, defence, no)
    [SerializeField] public string Name = ""; //name of the card
    [SerializeField] public string Effect = ""; //effect of the card
    [SerializeField] public string Description = ""; //description of the card
    [SerializeField] public int Health = 0; //health points
    [SerializeField] public int Damage = 0; //damage deal with the attack
    [SerializeField] public int Speed = 0; //attack speed
    [SerializeField] public int Weight = 0; //weight of the card
    [SerializeField] public string Species = ""; //species of the card (umano, nano, gigante, pianta, animale, spirito, concetto, magia, edificio)
    [SerializeField] public string Class = ""; //class of the card (barbarian, bard, cleric, druid, warrior, mage, hermit)
    [SerializeField] public string Nature = "";  //nature that addict the card (pericoloso, aggressivo, bilanciato, moderato, pacifista)
    [SerializeField] public Sprite Image; //image (artwork)
    [SerializeField] public int Id = 0; //univoc id
    
    //management
    public string team = ""; // player, enemy
    public string position = ""; // deck, hand, attack, defense
    public string _currentPos = ""; // hand, attack, defense
    public int placement = 0; //current placement (1-9)
    public bool isAttacking = false;
    public bool isDefending = false;
    public bool isMoving = false;
    
    //game logic
    public int ttl = 0; //number of turns from when the card was played
    public bool isStopped = false; //check if you can use the card
    public string stopPhase = ""; //phase when the card was used
    public bool hasAction = false; //variable to check if the card can do an action during attack phase

    //variable to check if you selected an item with the attack to avoid to bug the attackers queue
    public bool isTargetLocated = false;
    public int target = 0;

    //game manager
    public GameManager gameManager;

    //pointer
    [SerializeField] public GameObject pointer;
    public GameObject current_target;

    //watch a card
    public Collider2D OverlappingCollider;
    public BoxCollider2D Card_Collider;

    //drag and drop
    private Vector2 difference = Vector2.zero;
    private Vector2 start_position = Vector2.zero;
    public GameObject[] player_valid_defense_positions = new GameObject[4];
    public GameObject[] player_valid_attack_positions = new GameObject[5];
    private Vector2 newPos = Vector2.zero;
    private GameObject card_place; //place where you can place the card (omino)

    //enemy field
    public GameObject[] enemy_valid_defense_positions = new GameObject[4];
    public GameObject[] enemy_valid_attack_positions = new GameObject[5];

    void Start()
    {
        //find game manager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        player_valid_defense_positions = GameObject.FindGameObjectsWithTag("PlayerDefense");
        player_valid_attack_positions = GameObject.FindGameObjectsWithTag("PlayerAttack");
        enemy_valid_defense_positions = GameObject.FindGameObjectsWithTag("EnemyDefense");
        enemy_valid_attack_positions = GameObject.FindGameObjectsWithTag("EnemyAttack");
    }

    private void Update()
    {
        if (Health <= 0) //if the card die activate the place to position other cards and destroy the card
        {
            card_place.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void OnMouseDown() // mouse pressed
    {
        //use cards to defend
        if (gameManager.turn == "player" && team == "player" && gameManager.phase == "defense" && gameManager.turn_type == "play")
        {
            if (position == "defense" && gameManager.phase == "defense")
            {
                if (isDefending)
                {
                    isDefending = false;
                    gameManager.PlayerDefenders.Remove(placement);
                    isStopped = false;
                    stopPhase = "";
                }
                else
                {
                    isDefending = true;
                    gameManager.PlayerDefenders.Add(placement);
                    isStopped = true;
                    stopPhase= gameManager.phase;
                }
            }
        }

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
                        gameManager.PlayerAttacckers.Remove(placement);
                        gameManager.PlayerTargets.Remove(target);
                        target = 0;
                    }
                }
                else
                {
                    if (gameManager.PlayerAttacckers.Count < gameManager.Stress)
                    {
                        pointer.SetActive(true);
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
            if (position == "hand" && gameManager.phase == "attack")
            {
                isMoving = true;
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
            isMoving = false;
            //drop the card if it's in your hand and you have enough lymph
            if (position == "hand" && gameManager.Lymph >= Cost && newPos != Vector2.zero && _currentPos != Restriction)
            {
                gameManager.Lymph -= Cost;
                transform.position = newPos;
                position = _currentPos;
                card_place.SetActive(false);
                gameManager.PlayerFieldCards[placement-1] = gameObject;
            }
            else
            {
                if (position == "hand")
                {
                    transform.localScale = new Vector2(1, 1);
                    transform.SetPositionAndRotation(new Vector2(start_position.x, start_position.y), transform.rotation);
                }
            }

            //delete the pointer and select the cart to attack
            if (isAttacking)
            {
                //check if you select the enemy to attack
                try
                {
                    if (current_target.GetComponent<Card>().team != team)
                    {
                        target = current_target.GetComponent<Card>().placement;
                        gameManager.PlayerAttacckers.Add(placement);
                        gameManager.PlayerTargets.Add(target);
                        isTargetLocated = true;
                    }
                } catch { }

                pointer.transform.localPosition = Vector2.zero;
                pointer.SetActive(false);
            }

            if (!isTargetLocated)
            {
                isAttacking = false;
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        card_place = collision.gameObject;
        try
        {
            for (int i = 0; i < 5; i++)
            {
                if (collision.gameObject == player_valid_attack_positions[i])
                {
                    newPos = collision.gameObject.transform.position;
                    _currentPos = "attack";
                    placement = i + 5;
                    break;
                }
                if (collision.gameObject == player_valid_defense_positions[i])
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (position == "hand" && !collision.gameObject.GetComponent<Card>())
        {
            newPos = Vector2.zero;
            placement = 0;
        }
    }
    // card growing when overlapping
    private void OnMouseEnter()
    {
        OverlappingCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (position == "hand" && OverlappingCollider.gameObject == gameObject)
        {
            transform.localScale = new Vector2(2, 2);
            Card_Collider.size = new Vector2(40, 70);
            transform.SetPositionAndRotation(new Vector2(transform.position.x, transform.position.y + 1.2f), transform.rotation);
        }
    }
    
    private void OnMouseOver()
    {
        if (position == "hand" && OverlappingCollider.gameObject == gameObject)
        {
            if (isMoving)
            {
                transform.localScale = new Vector2(1, 1);
                Card_Collider.size = new Vector2(50, 75);
                transform.SetPositionAndRotation((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.rotation);
            }
        }
    }

    private void OnMouseExit()
    {
        if (position == "hand" && OverlappingCollider.gameObject == gameObject)
        {
            transform.localScale = new Vector2(1, 1);
            Card_Collider.size = new Vector2(50, 75);
            transform.SetPositionAndRotation(new Vector2(transform.position.x, transform.position.y - 1.2f), transform.rotation);
        }
    }

}
