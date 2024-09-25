using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlackJackHand : MonoBehaviour
{
    public int HandValue {  get; private set; }
    public int CardCount { get { return numCardsInHand; } }

    public bool isDealerHand;
    public GameObject HandValueDisplay;

    private CardObject[] cards;
    private int numCardsInHand;
    private int UnconvertedAceCount;
    private TMP_Text handValueDisplayText;
    private string handValueDisplayText_prompt;

    void Start()
    {
        cards = GetComponentsInChildren<CardObject>();
        Debug.Assert(cards != null && cards.Length > 0, "Missing Card Objects!");
        numCardsInHand = cards.Length;
        UnconvertedAceCount = 0;
        HandValue = 0;
        if(HandValueDisplay != null)
        {
            handValueDisplayText = HandValueDisplay.GetComponent<TMP_Text>();
            if(isDealerHand)
            {
                handValueDisplayText_prompt = "Dealer's Hand: ";
            }
            else
            {
                handValueDisplayText_prompt = "Player's Hand: ";
            }
        }
        ResetHand();
    }

    public void ResetHand()
    {
        for(int i = 0; i < numCardsInHand; i++)
        {
            cards[i].DiscardCard();
            cards[i].gameObject.SetActive(false);
        }
        numCardsInHand = 0;
        UnconvertedAceCount = 0;
        HandValue = 0;
        if (HandValueDisplay != null)
        {
            HandValueDisplay.SetActive(false);
        }
    }

    public void DrawCard(bool HideCard = false)
    {
        Debug.Assert(numCardsInHand < cards.Length);
        CardInfo newCardInformation = DeckManager.Instance.DrawCard();
        cards[numCardsInHand].gameObject.SetActive(true);
        cards[numCardsInHand].SetCard(newCardInformation, true, HideCard);
        numCardsInHand++;

        if(newCardInformation.Card_Rank == E_CARD_RANKS.RANK_A)
        {
            UnconvertedAceCount++;
        }
        HandValue += GetValueForRank(newCardInformation.Card_Rank);
        UpdateHandValue();
    }

    public void RevealCards()
    {
        for(int i = 0; i < numCardsInHand; i++)
        {
            cards[i].SetCard(cards[i].Card_Information, true, false);
        }
        HandValueDisplay.SetActive(true);
    }

    public void UpdateHandValue()
    {
        while(HandValue > BlackJackManager.BLACK_JACK_LIMIT && UnconvertedAceCount > 0)
        {
            HandValue -= 10;
            UnconvertedAceCount -= 1;
        }
        if (handValueDisplayText != null)
        {
            if(!HandValueDisplay.activeInHierarchy && !isDealerHand)
            {
                // If this is the player's hand, and we're not currently visible, make us visible.
                HandValueDisplay.SetActive(true);
            }
            handValueDisplayText.SetText(handValueDisplayText_prompt + HandValue.ToString());
        }
    }

    // In theory, we shouldn't need this function, but I have left it here, in case I would want it in future implementations.
    public int CountAces()
    {
        int count = 0;
        for(int i = 0; i < numCardsInHand; i++)
        {
            if (cards[i].Card_Rank == E_CARD_RANKS.RANK_A)
            {
                count++;
            }
        }
        return count;
    }

    private int GetValueForRank(E_CARD_RANKS cardRank)
    {
        switch(cardRank)
        {
            case E_CARD_RANKS.RANK_2:
                return 2;
            case E_CARD_RANKS.RANK_3:
                return 3;
            case E_CARD_RANKS.RANK_4:
                return 4;
            case E_CARD_RANKS.RANK_5:
                return 5;
            case E_CARD_RANKS.RANK_6:
                return 6;
            case E_CARD_RANKS.RANK_7:
                return 7;
            case E_CARD_RANKS.RANK_8:
                return 8;
            case E_CARD_RANKS.RANK_9:
                return 9;
            case E_CARD_RANKS.RANK_10:
            case E_CARD_RANKS.RANK_J:
            case E_CARD_RANKS.RANK_Q:
            case E_CARD_RANKS.RANK_K:
                return 10;
            case E_CARD_RANKS.RANK_A:
                return 11;
            default:
                Debug.Assert(false, "Error with parsing card!");
                return -1;
        }
    }
}
