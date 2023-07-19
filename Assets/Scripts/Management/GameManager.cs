using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //permanent GUI
    [SerializeField] public TextMeshProUGUI player_life;
    [SerializeField] public TextMeshProUGUI enemy_life;
    [SerializeField] public TextMeshProUGUI player_name;
    [SerializeField] public TextMeshProUGUI enemy_name;
    [SerializeField] public TextMeshProUGUI turn_text;
    [SerializeField] public TextMeshProUGUI phase_text;
    [SerializeField] public TextMeshProUGUI lymph_text;
    [SerializeField] public TextMeshProUGUI stress_text;

    //choose GUI
    [SerializeField] private GameObject choose_panel;
    [SerializeField] private GameObject hideable_choose_panel;
    [SerializeField] private GameObject button_lymph;
    [SerializeField] private GameObject button_stress;
    [SerializeField] private GameObject button_card;
    [SerializeField] private GameObject button_play;

    [SerializeField] public Transform player_canvas;
    [SerializeField] public Transform enemy_canvas;

    //players name
    public string PlayerName = "";
    public string EnemyName = "";

    //players life
    public int PlayerLife = 0;
    public int EnemyLife = 0;

    //number of card in each deck
    public int PlayerMaximumDeckCard = 30;
    public int EnemyMaximumDeckCard = 30;

    //turn action
    public string turn_type = "";

    //dynamic array to store decks
    public List<GameObject> PlayerDeck = new List<GameObject>();
    public List<GameObject> EnemyDeck = new List<GameObject>();

    //dynamic array to store hands
    public List<GameObject> PlayerHand = new List<GameObject>();
    public List<GameObject> EnemyHand = new List<GameObject>();

    //static array to store fields
    public GameObject[] PlayerFieldCards = new GameObject[9];
    public GameObject[] EnemyFieldCards = new GameObject[9];

    //dynamic array to manage player attacking turn
    public List<int> PlayerAttacckers = new List<int>();
    public List<int> PlayerTargets = new List<int>();
    public List<int> PlayerDefenders = new List<int>();

    //dynamic array to manage enemy attack turn
    public List<int> EnemyAttackers = new List<int>();
    public List<int> EnemyTargets = new List<int>();
    public List<int> enemyDefenders = new List<int>();

    //hand placement
    private GameObject newCard; //temp card object
    private float playerHandPosX = 0; //coordinates to spawn player card
    public int playerHandCards = 0; //number of card to give to the player at the start of the game
    private float enemyHandPosX = 0; //coordinates to spawn enemy card
    public int enemyHandCards = 0; //number of card to give to the enemy at the start of the game

    //game stats
    public int MaxLymph = 0;
    public int MaxStress = 0;
    public int Lymph = 0;
    public int Stress = 0;

    //turn management
    public string turn = ""; // player, enemy
    public string phase = ""; // attack, defense

    //DEBUG
    [SerializeField] private GameObject enemy_first_position;

    void Start()
    {
        //ONLY FOR DEBUG ON SINGLEPLAYER
        phase_text.SetText("defense");
        turn_text.SetText("player");

        //create the deck and hide it
        for (int i = 0; i < PlayerMaximumDeckCard; i++)
        {
            PlayerDeck.Add((GameObject)Resources.Load(Random.Range(1,4).ToString()));
            PlayerDeck[i].GetComponent<Card>().team = "player";
            PlayerDeck[i].GetComponent<Card>().position = "deck";
            PlayerDeck[i].SetActive(false);
        }

        //put cards into hand
        DrawCards(7);
        //AdjustHandGUI();

        //DEBUG ENEMY
        GameObject newEnemy = (GameObject)Instantiate(Resources.Load("4"), enemy_canvas);
        newEnemy.transform.position = enemy_first_position.transform.position;
        enemy_first_position.SetActive(false);
        newEnemy.GetComponent<Card>().team = "enemy";
        newEnemy.GetComponent<Card>().position = "attack";
        newEnemy.GetComponent<Card>().placement = enemy_first_position.GetComponent<Placement>().place;

        //GUI update
        if (turn == "player" && phase == "defense") 
        {
            choose_panel.SetActive(true);
        }

        Lymph = MaxLymph;
        Stress = MaxStress;
    }

    private void FixedUpdate() //Update GUI and statistics
    {
        if (PlayerLife <= 0)
        {
            Debug.Log("You Lose!");
        }
        if (EnemyLife <= 0)
        {
            Debug.Log("You Win!");
        }

        player_life.text = PlayerLife.ToString();
        enemy_life.text = EnemyLife.ToString();
        player_name.text = PlayerName.ToString();
        enemy_name.text = EnemyName.ToString();
        lymph_text.text = "Lymph: " + Lymph.ToString();
        stress_text.text = "Stress: " + Stress.ToString();
    }

    public void PassTurn()
    {
        if (turn == "player")
        {
            if (phase == "defense")
            {
                phase = "attack";
                phase_text.SetText("attack");
            }
            else
            {
                turn = "enemy";
                phase = "defense";
                phase_text.SetText("defense");
                turn_text.SetText("enemy");
                Lymph = MaxLymph;
                Stress = MaxStress;
            }
        }
        else
        {
            if (phase == "defense")
            {
                phase = "attack";
                phase_text.SetText("attack");
            }
            else
            {
                turn = "player";
                phase = "defense";
                phase_text.SetText("defense");
                turn_text.SetText("player");
                Lymph = MaxLymph;
                Stress = MaxStress;

                choose_panel.SetActive(true);

                CheckCard();
                CheckLymph();
                CheckStress();
                CheckPlay();
            }
        }

        for (int i = 0; i < PlayerFieldCards.Length; i++) //update things related to turn changing
        {
            if (PlayerFieldCards[i] != null)
            {
                PlayerFieldCards[i].SendMessage("ChangeTurnAndPhase", new string[] {turn, phase});
            }
        }
    }

    public void DrawCards(int n) //put card in hand from the deck (n = number of cards)
    {
        while(playerHandCards + n > 10) //don't draw you reached maximum limit of card
        {
            n--;
        }

        while (n > PlayerDeck.Count) //don't draw if there aren't cards anymore
        {
            n--;
        }

        playerHandCards += n;

        for (int i = 0; i < n ; i++)
        {
            PlayerDeck[i].GetComponent<Card>().position = "hand";
            PlayerDeck[i].SetActive(true);
            PlayerHand.Add(PlayerDeck.ToArray()[i]); //add card to the hand
            PlayerHand[^1] = Instantiate(PlayerHand[^1], player_canvas);
            PlayerDeck.RemoveAt(i); //remove card from the deck
        }

        AdjustHandGUI();
    }

    public void AdjustHandGUI()
    {
        if (playerHandCards == 0) //no card
        {
            return;
        }

        for (int i = 0; i < playerHandCards; i++)
        {
            PlayerHand[i].transform.localPosition = new Vector2((i * 50) - (playerHandCards * 25) + (playerHandCards % 2 == 0 ? 50:25) + 25, -205);
        }
        
        
    }

    public void AddLymph() //async funct (button)
    {
        Lymph++;
        MaxLymph++;
        choose_panel.SetActive(false);
        turn_type = "lymph";
    }

    public void AddStress() //async funct (button)
    {
        Stress++;
        MaxStress++;
        choose_panel.SetActive(false);
        turn_type = "stress";
    }

    public void PlayTurn() //async funct (button)
    {
        choose_panel.SetActive(false);
        turn_type = "play";
    }

    public void DrawCard() //async funct (button)
    {
        DrawCards(1);
        choose_panel.SetActive(false);
        turn_type = "draw";
        
    }

    public void CheckLymph()
    {
        //if you reached the lymph limit you can't increase it anymore
        if (MaxLymph >= 10)
        {
            button_lymph.GetComponent<Button>().enabled = false;
        }
        else
        {
            button_lymph.GetComponent<Button>().enabled = true;
        }
    }
    public void CheckStress()
    {
        //if you reached the stress limit you can't increase it anymore
        if (MaxStress >= 5)
        {
            button_stress.GetComponent<Button>().enabled = false;
        }
        else
        {
            button_stress.GetComponent<Button>().enabled = true;
        }
    }
    public void CheckPlay()
    {

    }
    public void CheckCard()
    {
        //if you finish card in the deck you can't draw anymore
        if (playerHandCards >= 10)
        {
            button_card.GetComponent<Button>().enabled = false;
        }
        else
        {
            button_card.GetComponent<Button>().enabled = true;
        }
    }

    public void HideAndShowChooseGUI()
    {
        hideable_choose_panel.SetActive(!hideable_choose_panel.activeSelf);
    }

    public void AttackPlayer()
    {
        //bisogna riordinarli in base alla speed delle varie carte

        EnemyAttackers.Sort(); //enemy that attacks
        EnemyTargets.Sort(); //player cards that is being attacked
        PlayerDefenders.Sort(); //player cards that defends

        for (int i = 0; i < EnemyAttackers.Count; i++)
        {

        }
    }
}
