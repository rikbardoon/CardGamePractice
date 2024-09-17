using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using System;

public enum WINNING_HANDS
{
    ROYAL_FLUSH=0,
    STRAIGHT_FLUSH,
    4_OF_A_KIND,
    FULL_HOUSE,
    FLUSH,
    STRAIGHT,
    3_OF_A_KIND,
    TWO_PAIR,
    JACKS_OR_BETTER,
    NO_WIN,
    WINNING_HAND_COUNT
}

public class PokerGameManager : MonoBehaviour
{
    private enum GameState
    {
        GAME_IDLE = 0,
        GAME_STARTED,
        GAME_EVALUATING,
        STATE_COUNT
    }

    List<CardInfo> playerHand;
    GameState state;
    private CardObject[] cards;
    public TMP_Text ResultText;
    public bool ReshuffleBetweenGames = false;

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.GAME_IDLE;
        cards = GetComponentsInChildren<CardObject>();
        playerHand = new List<CardInfo>();
        ResultText.SetText("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGamePressed()
    {
        // Deal Hand to player and update buttons.
        UnityEngine.Debug.Log("Start Game button clicked.");

        if (state == GameState.GAME_IDLE)
        {
            playerHand.Clear();
            for (int i = 0; i < cards.Length; i++)
            {
                CardInfo drawnCard = DeckManager.Instance.DrawCard();
                playerHand.Add(drawnCard);
                cards[i].SetCard(drawnCard);
            }
            ResultText.SetText("");
            state = GameState.GAME_STARTED;
        }
    }

    public void DrawPressed()
    {
        UnityEngine.Debug.Log("Draw button clicked.");
        // Discard In Play cards, draw new cards, and evaluate win. 
        // Update buttons.
        if(state == GameState.GAME_STARTED)
        {
            for(int i = 0; i < cards.Length; i++)
            {
                if (playerHand[i] != null && !playerHand[i].Poker_Hold)
                {
                    DeckManager.Instance.PutCardInDiscardPile(playerHand[i]);
                    CardInfo newCard = DeckManager.Instance.DrawCard();
                    playerHand[i] = newCard;
                    cards[i].SetCard(newCard);
                }
            }
            state = GameState.GAME_EVALUATING;
        }
        EvaluateHand();
    }

    public void EvaluateHand()
    {
        if(state == GameState.GAME_EVALUATING)
        {
            // Code to evaluate the hand.
            {
            }

            ResultText.SetText("Result");
            state = GameState.GAME_IDLE;
        }
        if(ReshuffleBetweenGames)
        {
            DeckManager.Instance.ShuffleDiscardToDeck();
        }
    }

    public void ChangeGamePressed()
    {
        // Change to main screen for game selection.
        UnityEngine.Debug.Log("Change Game button pressed");
    }
}

public class PokerEvaluator
{
    private CardInfo[] Hand;
    private int[] RankCount;
    private int[] SuitCount;

    private class CardComparer : IComparer<CardInfo>
    {
        public int Compare(CardInfo x, CardInfo y)
        {
            if (x.Card_Rank < y.Card_Rank)
            {
                return -1;
            }
            else if(x.Card_Rank > y.Card_Rank)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    // First, we'll sort the hand into ascending rank order.
    // Count the number of re-occurring ranks
    // Count the number of each suit.
    // Compare to each hand.
    public PokerEvaluator()
    {
    }

    public WINNING_HANDS EvaluateHand(CardInfo[] inHand)
    {
        // Make new arrays to reset the counts to 0.
        RankCount = new int[E_CARD_RANKS.RANK_COUNT];
        SuitCount = new int[E_CARD_SUITS.SUIT_COUNT];
        Hand = inHand;

        SortHandByRank();
        CountRanks();
        CountSuits();
        WINNING_HANDS result = DetermineHand();
        return result;
    }

    public void SortHandByRank()
    {
        CardComparer cardComparer = new CardComparer();
        Array.Sort(Hand, cardComparer);
    }

    public void CountRanks()
    {
        foreach(var card in Hand)
        {
            RankCount[(int)(card.Card_Rank)]++;
        }
    }

    public void CountSuits()
    {
        foreach (var card in Hand)
        {
            SuitCount[(int)(card.Card_Suit)]++;
        }
    }

    /*
 * public enum WINNING_HANDS
{
    ROYAL_FLUSH=0,
    STRAIGHT_FLUSH,
    4_OF_A_KIND,
    FULL_HOUSE,
    FLUSH,
    STRAIGHT,
    3_OF_A_KIND,
    TWO_PAIR,
    JACKS_OR_BETTER,
    WINNING_HAND_COUNT
}
 */
    public WINNING_HANDS DetermineHand()
    {
        bool flush = HasFlush();
        bool straight = HasStraight();

        if(flush && straight)
        {
            if ((RankCount[(int)(E_CARD_RANKS.RANK_10)] == 1) && (RankCount[(int)(E_CARD_RANKS.RANK_A)] == 1))
            {
                // If we have a straight and it includes a 10 and an A, then we know it's a Royal Straight.
                return WINNING_HANDS.ROYAL_FLUSH;
            }
            else
            {
                return WINNING_HANDS.STRAIGHT_FLUSH;
            }
        }
        // check 4 of a kind.
        // check full house
        if(flush)
        {
            return WINNING_HANDS.FLUSH;
        }
        if(straight)
        {
            return WINNING_HANDS.STRAIGHT;
        }
        // check 3 of a kind
        // check two pair.
        // check pair.

        return WINNING_HANDS.NO_WIN;
    }

    private int GetNthMostOccuringCard(int n)
    {
        if (n == 0)
            return -1;

        for(int rank = 0; rank < RankCount.Length; rank++)
        {

        }
    }

    private bool HasStraight()
    {
        bool result = true;
        int nCards = Hand.Length;

        for (int i = 0; i < RankCount.Length; i++)
        {
            if (RankCount[i] == 1) 
            {
                nCards--;
            }
            else if (RankCount[i] > 1)
            {
                // if > 1, then we know we can't have a straight.
                break;
            }
            else if(nCards < Hand.Length)
            {
                // We found a card, but aren't finding one now. 
                // Either we found all five cards, or we just broke the straight and didn't find an ascending card.
                break;
            }
        }
        result = nCards == 0; // If we found all 5 cards, then we found a straight.
        return result;
    }

    private bool HasFlush()
    {
        for(int i = 0; i < SuitCount.Length; i++)
        {
            if (SuitCount[i] == Hand.Length)
            {
                return true;
            }
        }
        return false;
    }
}
