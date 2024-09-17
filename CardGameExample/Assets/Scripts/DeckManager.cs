using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    static public DeckManager Instance { get; private set; }

    private Stack<CardInfo> Deck;
    private List<CardInfo> DiscardPile;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        DeckManager.Instance.Reset();
        DebugPrintDeck();
    }

    public void Reset()
    {
        Deck = new Stack<CardInfo>();
        DiscardPile = new List<CardInfo>();

        int numCardOptions = (int)(E_CARD_SUITS.SUIT_COUNT) * (int)(E_CARD_RANKS.RANK_COUNT);

        // Add cards to discard pile.
        for(int i = 0; i < numCardOptions; i++)
        {
            CardInfo newCard = new CardInfo(i);
            DiscardPile.Add(newCard);
        }

        // Shuffle Discard into Deck.
        ShuffleDiscardToDeck();
    }

    public void ShuffleDiscardToDeck()
    {
        UnityEngine.Debug.Assert(Deck != null, "Deck is uninitialized.");
        UnityEngine.Debug.Assert(DiscardPile != null, "Discard Pile is uninitialized.");
        UnityEngine.Debug.Assert(Deck.Count() > 0 || DiscardPile.Count() > 0, "No cards in Deck or Discard Pile");

        // Take what remains in the deck and put it in the discard pile.
        while(Deck.Count() > 0)
        {
            DiscardPile.Add(Deck.Pop());
        }

        System.Random rnd = new System.Random(DateTime.Now.Millisecond);

        while(DiscardPile.Count() > 0)
        {
            int selectedCardIndex = rnd.Next(0, DiscardPile.Count());
            CardInfo selectedCard = DiscardPile[selectedCardIndex];
            selectedCard.Card_State = E_CARD_STATE.STATE_IN_DECK;
            Deck.Push(selectedCard);
            DiscardPile.RemoveAt(selectedCardIndex);
        }
    }

    public CardInfo DrawCard()
    {
        if(Deck.Count <= 0)
        {
            ShuffleDiscardToDeck();
        }

        CardInfo DrawnCard = Deck.Pop();
        DrawnCard.Card_State = E_CARD_STATE.STATE_IN_PLAY;

        return DrawnCard;
    }

    public void PutCardInDiscardPile(CardInfo card)
    {
        UnityEngine.Debug.Assert(!DiscardPile.Contains(card), "Card " + card.To_String() + " already in deck.");
        card.Card_State = E_CARD_STATE.STATE_IN_DISCARD;
        card.Poker_Hold = false;
        DiscardPile.Add(card);
    }

    private void DebugPrintDeck()
    {
        List<CardInfo> cardList = Deck.ToList();

        string logMsg = "Deck output\n";
        logMsg += "Deck Size: " + Deck.Count().ToString() + "\n";
        logMsg += "Deck Components:\n";

        for(int i = 0; i < cardList.Count; i++)
        {
            logMsg += cardList[i].To_String(true) + "\n";
        }

        UnityEngine.Debug.Log(logMsg);
    }
}
