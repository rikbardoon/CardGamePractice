using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class CardObject : MonoBehaviour
{
    public E_CARD_SUITS Card_Suit { get; private set; }
    public E_CARD_RANKS Card_Rank { get; private set; }

    public TMP_Text CardText;
    public GameObject HoldIndicator;

    public CardInfo Card_Information;

    private RawImage Card_Image;

    void Awake()
    {
        Card_Image = gameObject.GetComponent<RawImage>();
    }

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

    public void SetCard(CardInfo cardInfo, bool short_text = false, bool HideCard = false)
    {
        Card_Information = cardInfo;
        Card_Suit = cardInfo.Card_Suit;
        Card_Rank = cardInfo.Card_Rank;

        // Update Visuals
        if (Card_Image != null)
        {
            if(HideCard)
            {
                Card_Image.texture = DeckManager.CardBackTexture;
            }
            else
            {
                Card_Image.texture = cardInfo.Card_Image;
            }
            // Since we have an image, we don't need the text.
            CardText.gameObject.SetActive(false);
        }
        else if (CardText != null) // Since we don't have an image, we'll just use text instead.
        {
            UnityEngine.Debug.LogWarning("No RawImage found! Defaulting to text instead.");
            if(HideCard)
            {
                CardText.text = "Hidden";
            }
            else if(!short_text)
            {
                CardText.text = cardInfo.To_String();
            }
            else
            {
                CardText.text = cardInfo.ToShortString();
            }
        }
        if(HoldIndicator != null)
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

    public void DiscardCard()
    {
        if(Card_Information == null)
        {
            // Do nothing, as we have no card presently.
            return;
        }
        if(Card_Information.Card_State != E_CARD_STATE.STATE_IN_DISCARD)
        {
            DeckManager.Instance.PutCardInDiscardPile(Card_Information);
        }
        Card_Information = null;
    }

    public void CardPressed()
    {
        if (Card_Information != null)
        {
            SetHold(!Card_Information.Poker_Hold);
        }
    }
}
