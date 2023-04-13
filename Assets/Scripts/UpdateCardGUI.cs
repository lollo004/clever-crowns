using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCardGUI : MonoBehaviour
{
    [SerializeField] private Message card;

    [SerializeField] private Image Icon; //image (artwork)
    [SerializeField] private TMP_Text Cost; //cost of the card
    [SerializeField] private Image Type; //type of the card (troop, magic, building)
    [SerializeField] private Image Restriction; //position in the field (attack, defence, no)
    [SerializeField] private TMP_Text Name; //name of the card
    [SerializeField] private TMP_Text Effect; //effect of the card
    [SerializeField] private TMP_Text Description; //description of the card
    [SerializeField] private TMP_Text Health; //health points
    [SerializeField] private TMP_Text Damage; //damage deal with the attack
    [SerializeField] private TMP_Text Speed; //attack speed
    [SerializeField] private TMP_Text Weight; //weight of the card
    [SerializeField] private TMP_Text Nature;  //nature that addict the card (pericoloso, aggressivo, bilanciato, moderato, pacifista)
    [SerializeField] private TMP_Text Class; //class of the card (barbarian, bard, cleric, druid, warrior, mage, hermit)
    [SerializeField] private TMP_Text Species; //species of the card (umano, nano, gigante, pianta, animale, spirito, concetto, magia, edificio)
    [SerializeField] private GameObject ShowMore; //panel to show more of the card

    //UpdateGUIAtLatest
    public void FixedUpdate()
    {
        Icon.sprite = card.Image;
        Cost.text = card.Cost.ToString();

        //da mettere a posto in base agli sprite
        //Type.sprite = card.Cost.ToString();
        //Restriction.sprite = card.Cost.ToString();
        
        Name.text = card.Name.ToString();
        Effect.text = card.Effect.ToString();
        Description.text = card.Description.ToString();
        Health.text = card.Health.ToString();
        Damage.text = card.Damage.ToString();
        Speed.text = card.Speed.ToString();
        Weight.text = card.Weight.ToString();
        Nature.text = card.Nature.ToString();
        Class.text = card.Class.ToString();
        Species.text = card.Species.ToString();
    }

    public void ShowMoreDetails()
    {
        ShowMore.SetActive(true);
    }

    public void ShowLessDetails()
    {
        ShowMore.SetActive(false);
    }
}
