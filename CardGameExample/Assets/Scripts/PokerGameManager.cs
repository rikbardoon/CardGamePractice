using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security;

public enum WINNING_HANDS
{
    ROYAL_FLUSH=0,
    STRAIGHT_FLUSH,
    FOUR_OF_A_KIND,
    FULL_HOUSE,
    FLUSH,
    STRAIGHT,
    THREE_OF_A_KIND,
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
    private PokerEvaluator evaluator;

    public TMP_Text ResultText;
    public bool ReshuffleBetweenGames = false;

    public Button StartGameButton;
    public Button DrawButton;
    public Button ChangeGameButton;

    // Start is called before the first frame update
    void Start()
    {
        cards = GetComponentsInChildren<CardObject>();
        playerHand = new List<CardInfo>();
        ResultText.SetText("");
        evaluator = new PokerEvaluator();
        SetGameState(GameState.GAME_IDLE);
    }

    private void SetGameState(GameState newState)
    {
        state = newState;
        // Update buttons.
        if(StartGameButton != null)
        {
            StartGameButton.interactable = state == GameState.GAME_IDLE;
        }
        if (DrawButton != null) 
        {
            DrawButton.interactable = state == GameState.GAME_STARTED;
        }
        if (ChangeGameButton != null)
        {
            // Only allow Change Button to show when not in the middle of a game.
            ChangeGameButton.interactable = state == GameState.GAME_IDLE;
        }
    }

    public void StartGamePressed()
    {
        // Deal Hand to player and update buttons.
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
            SetGameState(GameState.GAME_STARTED);
        }
    }

    public void DrawPressed()
    {
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
            SetGameState(GameState.GAME_EVALUATING);
        }
        EvaluateHand();
    }

    public void EvaluateHand()
    {
        if(state == GameState.GAME_EVALUATING)
        {
            // Code to evaluate the hand.
            WINNING_HANDS handResult = evaluator.EvaluateHand(playerHand.ToArray());

            //If we wanted to implement monetary components to this game, we would add the credit rewards to the player here.
            // We would also change the below to show the winning hand on the paytable.

            ResultText.SetText(PokerEvaluator.GetWinningHandString(handResult));
            SetGameState(GameState.GAME_IDLE);
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

    public static string GetWinningHandString(WINNING_HANDS hand_result)
    {
        switch(hand_result)
        {
            case WINNING_HANDS.ROYAL_FLUSH:
                return "Royal Flush";
            case WINNING_HANDS.STRAIGHT_FLUSH:
                return "Straight Flush";
            case WINNING_HANDS.FOUR_OF_A_KIND:
                return "4 Of A Kind";
            case WINNING_HANDS.FULL_HOUSE:
                return "Full House";
            case WINNING_HANDS.FLUSH:
                return "Flush";
            case WINNING_HANDS.STRAIGHT:
                return "Straight";
            case WINNING_HANDS.THREE_OF_A_KIND:
                return "3 Of A Kind";
            case WINNING_HANDS.TWO_PAIR:
                return "Two Pair";
            case WINNING_HANDS.JACKS_OR_BETTER:
                return "Jacks Or Better";
            case WINNING_HANDS.NO_WIN:
                return "Game Over";
            default:
                return "Unknown";
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
        Hand = inHand;

        SortHandByRank();
        WINNING_HANDS result = DetermineHand();
        return result;
    }

    public void SortHandByRank()
    {
        CardComparer cardComparer = new CardComparer();
        Array.Sort(Hand, cardComparer);
    }

    public WINNING_HANDS DetermineHand()
    {
        bool flush = HasFlush();
        bool straight = HasStraight();

        if(flush && straight)
        {
            if (Hand[0].Card_Rank == E_CARD_RANKS.RANK_10 && Hand[Hand.Length-1].Card_Rank == E_CARD_RANKS.RANK_A)
            {
                // If we have a straight and it includes a 10 and an A, then we know it's a Royal Straight.
                return WINNING_HANDS.ROYAL_FLUSH;
            }
            else
            {
                return WINNING_HANDS.STRAIGHT_FLUSH;
            }
        }
        if (Hand[0].Card_Rank == Hand[3].Card_Rank || Hand[1].Card_Rank == Hand[4].Card_Rank)
        {
            return WINNING_HANDS.FOUR_OF_A_KIND;
        }
        if ((Hand[0].Card_Rank == Hand[2].Card_Rank && Hand[3].Card_Rank == Hand[4].Card_Rank) ||
            (Hand[0].Card_Rank == Hand[1].Card_Rank && Hand[2].Card_Rank == Hand[4].Card_Rank))
        {
            return WINNING_HANDS.FULL_HOUSE;
        }
        if(flush)
        {
            return WINNING_HANDS.FLUSH;
        }
        if(straight)
        {
            return WINNING_HANDS.STRAIGHT;
        }
        if (Hand[0].Card_Rank == Hand[2].Card_Rank || Hand[1].Card_Rank == Hand[3].Card_Rank || Hand[2].Card_Rank == Hand[4].Card_Rank)
        {
            return WINNING_HANDS.THREE_OF_A_KIND;
        }
        if ((Hand[0].Card_Rank == Hand[1].Card_Rank && Hand[2].Card_Rank == Hand[3].Card_Rank) ||
            (Hand[0].Card_Rank == Hand[1].Card_Rank && Hand[3].Card_Rank == Hand[4].Card_Rank) ||
            (Hand[1].Card_Rank == Hand[2].Card_Rank && Hand[3].Card_Rank == Hand[4].Card_Rank))
        {
            return WINNING_HANDS.TWO_PAIR;
        }
        if(Hand[0].Card_Rank == Hand[1].Card_Rank && (int)(Hand[0].Card_Rank) >= (int)(E_CARD_RANKS.RANK_J))
        {
            return WINNING_HANDS.JACKS_OR_BETTER;
        }
        if (Hand[1].Card_Rank == Hand[2].Card_Rank && (int)(Hand[1].Card_Rank) >= (int)(E_CARD_RANKS.RANK_J))
        {
            return WINNING_HANDS.JACKS_OR_BETTER;
        }
        if (Hand[2].Card_Rank == Hand[3].Card_Rank && (int)(Hand[2].Card_Rank) >= (int)(E_CARD_RANKS.RANK_J))
        {
            return WINNING_HANDS.JACKS_OR_BETTER;
        }
        if (Hand[3].Card_Rank == Hand[4].Card_Rank && (int)(Hand[3].Card_Rank) >= (int)(E_CARD_RANKS.RANK_J))
        {
            return WINNING_HANDS.JACKS_OR_BETTER;
        }

        return WINNING_HANDS.NO_WIN;
    }

    private bool HasStraight()
    {
        bool result = true;

        for(int i = 0; i < Hand.Length-1; i++)
        {
            if ((int)(Hand[i].Card_Rank) != (int)(Hand[i + 1].Card_Rank + 1))
            {
                result = false;
                break;
            }
        }

        return result;
    }

    private bool HasFlush()
    {
        bool result = true;

        for(int i = 0; i < Hand.Length-1; i++)
        {
            if (Hand[i].Card_Suit != Hand[i+1].Card_Suit)
            {
                result = false;
                break;
            }
        }
        return result;
    }
}
