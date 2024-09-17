using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_CARD_SUITS
{
    SUIT_CLUB = 0,
    SUIT_DIAMOND,
    SUIT_SPADES,
    SUIT_HEARTS,
    SUIT_COUNT,
    SUIT_UNSET = -1
}

public enum E_CARD_RANKS
{
    RANK_2 = 0,
    RANK_3,
    RANK_4,
    RANK_5,
    RANK_6,
    RANK_7,
    RANK_8,
    RANK_9,
    RANK_10,
    RANK_J,
    RANK_Q,
    RANK_K,
    RANK_A,
    RANK_COUNT,
    RANK_UNSET = -1
}

public enum E_CARD_STATE
{
    STATE_IN_DECK = 0,
    STATE_IN_PLAY,
    STATE_IN_DISCARD,
    STATE_COUNT,
    STATE_UNSET = -1
}

public class CardInfo
{
    public int card_id;
    public E_CARD_SUITS Card_Suit;
    public E_CARD_RANKS Card_Rank;
    public E_CARD_STATE Card_State;
    public bool Poker_Hold;

    public CardInfo(int cardId)
    {
        card_id = cardId;
        Card_Suit = (E_CARD_SUITS)(cardId / (int)E_CARD_RANKS.RANK_COUNT);
        Card_Rank = (E_CARD_RANKS)(cardId % (int)E_CARD_RANKS.RANK_COUNT);
        Card_State = E_CARD_STATE.STATE_IN_DECK;
        Poker_Hold = false;
    }

    public string To_String(bool FullDebug = false)
    {
        if(FullDebug)
        {
            return "Card" +
                " - ID: " + card_id.ToString() +
                " - Rank: " + RankToString(Card_Rank) + 
                " - Suit: " + SuitToString(Card_Suit) + 
                " - State: " + StateToString(Card_State) + 
                " - IsHeld: " + Poker_Hold.ToString();
        }
        return RankToString(Card_Rank) + " of " + SuitToString(Card_Suit);
    }

    public static string SuitToString(E_CARD_SUITS suit)
    {
        switch (suit)
        {
            case E_CARD_SUITS.SUIT_CLUB:
                return "Clubs";
            case E_CARD_SUITS.SUIT_DIAMOND:
                return "Diamonds";
            case E_CARD_SUITS.SUIT_SPADES:
                return "Spades";
            case E_CARD_SUITS.SUIT_HEARTS:
                return "Hearts";
            default:
                return "None";
        }
    }

    public static string RankToString(E_CARD_RANKS rank)
    {
        switch (rank)
        {
            case E_CARD_RANKS.RANK_A:
                return "A";
            case E_CARD_RANKS.RANK_2:
                return "2";
            case E_CARD_RANKS.RANK_3:
                return "3";
            case E_CARD_RANKS.RANK_4:
                return "4";
            case E_CARD_RANKS.RANK_5:
                return "5";
            case E_CARD_RANKS.RANK_6:
                return "6";
            case E_CARD_RANKS.RANK_7:
                return "7";
            case E_CARD_RANKS.RANK_8:
                return "8";
            case E_CARD_RANKS.RANK_9:
                return "9";
            case E_CARD_RANKS.RANK_10:
                return "10";
            case E_CARD_RANKS.RANK_J:
                return "J";
            case E_CARD_RANKS.RANK_Q:
                return "Q";
            case E_CARD_RANKS.RANK_K:
                return "K";
            default:
                return "None";
        }
    }

    public static string StateToString(E_CARD_STATE state)
    {
        switch (state)
        {
            case E_CARD_STATE.STATE_IN_DECK:
                return "In Deck";
            case E_CARD_STATE.STATE_IN_DISCARD:
                return "In Discard";
            case E_CARD_STATE.STATE_IN_PLAY:
                return "In Play";
            default:
                return "Not Set";
        }
    }
}
