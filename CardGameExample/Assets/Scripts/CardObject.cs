using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class CardObject : MonoBehaviour
{
    public E_CARD_SUITS Card_Suit { get; private set; }
    public E_CARD_RANKS Card_Rank { get; private set; }

    public TMP_Text CardText;
    public GameObject HoldIndicator;

    private CardInfo Card_Information;

    // Start is called before the first frame update
    void Start()
    {
        Card_Suit = E_CARD_SUITS.SUIT_UNSET;
        Card_Rank = E_CARD_RANKS.RANK_UNSET;
        if(HoldIndicator)
        {
            HoldIndicator.SetActive(false);
        }
    }

    public void SetCard(CardInfo cardInfo)
    {
        Card_Information = cardInfo;
        Card_Suit = cardInfo.Card_Suit;
        Card_Rank = cardInfo.Card_Rank;

        // Update Visuals
        if(CardText)
        {
            CardText.text = cardInfo.To_String();
        }
        if(HoldIndicator)
        {
            HoldIndicator.SetActive(cardInfo.Poker_Hold);
        }
    }

    public void SetHold(bool hold)
    {
        if(HoldIndicator)
        {
            HoldIndicator.SetActive(hold);
        }
        Card_Information.Poker_Hold = hold;
    }

    public void CardPressed()
    {
        if (Card_Information != null)
        {
            Card_Information.Poker_Hold = !Card_Information.Poker_Hold;
            SetHold(Card_Information.Poker_Hold);
        }
    }
}
