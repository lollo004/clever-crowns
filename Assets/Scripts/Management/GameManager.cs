using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //GUI
    [SerializeField] public TextMeshProUGUI player_life;
    [SerializeField] public TextMeshProUGUI enemy_life;
    [SerializeField] public TextMeshProUGUI player_name;
    [SerializeField] public TextMeshProUGUI enemy_name;
    [SerializeField] public TextMeshProUGUI turn_text;
    [SerializeField] public TextMeshProUGUI phase_text;

    //players name
    public string PlayerName = "";
    public string EnemyName = "";

    //players life
    public int PlayerLife = 30;
    public int EnemyLife = 0;

    //number of card in each deck
    public int PlayerDeckCard = 30;
    public int EnemyDeckCard = 0;

    //dynamic array to store decks
    public List<int> PlayerDeck = new List<int>();
    public List<int> EnemyDeck = new List<int>();

    //dynamic array to store the attacking cards
    public List<int> PlayerAttacckers = new List<int>();
    public List<int> EnemyAttackers = new List<int>();
    public int PlayerNumberOfAttackers = 0;
    public int EnemyNumberOfAttackers = 0;

    //card to attack
    public GameObject target;

    //hand placement
    public GameObject newCard; //card object
    public float handPosX = 0; //coordinates to spawn card
    public int handCards = 2; //number of card to give at the start of the game

    //game stats
    public int Lymph = 0;
    public int Stress = 0;
    public int Grace = 0;

    //turn management
    public string turn = ""; // player, enemy
    public string phase = ""; // attack, defense

    void Start()
    {
        //setting life on gui element
        player_life.text = PlayerLife.ToString();
        enemy_life.text = EnemyLife.ToString();
        player_name.text = PlayerName.ToString();
        enemy_name.text = EnemyName.ToString();
        phase_text.SetText("attack");
        turn_text.SetText("player");

        //random create a deck for player
        for (int i = 0; i < handCards; i++)
        {
            PlayerDeck.Add(Random.Range(1,4));
        }

        //generate first cards
        GenerateHand(1);

        GameObject newEnemy = (GameObject)Instantiate(Resources.Load("Card"));
        newEnemy.transform.position = new Vector3(3, 2, 0);
        newEnemy.GetComponent<Card>().team = "enemy";
        newEnemy.GetComponent<Card>().position = "attack";
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
            }
        }
    }

    //generate card based on number of card in the hand
    private void GenerateHand(int y) //y indicate spawn location (1 = player | -1 = enemy)
    {
        //set first coordinate based on number of card in the hand
        handPosX = 5 - ((handCards - 1) * 0.7f);
        //spawn all the cards on the right position
        for (int i = 0; i < handCards; i++)
        {
            newCard = (GameObject)Instantiate(Resources.Load(PlayerDeck[i].ToString()));
            newCard.transform.position = new Vector3(handPosX + (i*1.1f) - 2.5f, y * -5, 0);
            newCard.GetComponent<Card>().team = "player";
            newCard.GetComponent<Card>().position = "hand";
        }
    }
}
