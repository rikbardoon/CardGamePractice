using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Security;


public class BlackJackManager : MonoBehaviour
{
    public const int BLACK_JACK_LIMIT = 21;

    public TMP_Text Text_Result;

    public enum E_GAME_STATE
    {
        GAME_IDLE = 0,
        GAME_PLAYERS_TURN,
        GAME_DEALERS_TURN,
        STATE_COUNT
    }

    public BlackJackHand playerHand;
    public BlackJackHand dealerHand;

    public bool ReshuffleDeckBetweenHands = false;

    private E_GAME_STATE CurrentGameState;

    public Button StartGameButton;
    public Button HitButton;
    public Button StayButton;
    public Button ChangeGameButton;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(playerHand != null, "Missing player hand object!");
        Debug.Assert(dealerHand != null, "Missing dealer hand object!");
        if(Text_Result != null)
        {
            Text_Result.SetText("");
        }
        DeckManager.Instance.ResetDeck(); // Let's reset the deck manager whenever this game loads.
        SetGameState(E_GAME_STATE.GAME_IDLE);
    }

    public void ChangeGamePressed()
    {
        // Reset so the cards are discarded.
        ResetTable();
        // Change to main screen for game selection.
        SceneManager.LoadScene("SelectionScreen");
    }

    public void StartGamePressed()
    {
        if (CurrentGameState == E_GAME_STATE.GAME_IDLE)
        {
            ResetTable();
            playerHand.DrawCard(false);
            playerHand.DrawCard(false);

            dealerHand.DrawCard(true);
            dealerHand.DrawCard(false);

            if(playerHand.HandValue == 21 || dealerHand.HandValue == 21)
            {
                // BlackJack! Just go straight to evaluation.
                dealerHand.RevealCards();
                EvaluateWin();
                return;
            }
        }
        SetGameState(E_GAME_STATE.GAME_PLAYERS_TURN);
    }

    public void HitButtonPressed()
    {
        if(CurrentGameState == E_GAME_STATE.GAME_PLAYERS_TURN)
        {
            playerHand.DrawCard();
            if (playerHand.HandValue > 21)
            {
                // Player busts!
                dealerHand.RevealCards();
                EvaluateWin();
            }
        }
    }

    public void StayButtonPressed()
    {
        if(CurrentGameState == E_GAME_STATE.GAME_PLAYERS_TURN)
        {
            SetGameState(E_GAME_STATE.GAME_DEALERS_TURN);
            DealersTurn();
        }
    }

    public void SetGameState(E_GAME_STATE gameState)
    {
        CurrentGameState = gameState;
        // Update buttons.
        if (StartGameButton != null)
        {
            StartGameButton.interactable = CurrentGameState == E_GAME_STATE.GAME_IDLE;
        }
        if (HitButton != null)
        {
            HitButton.interactable = CurrentGameState == E_GAME_STATE.GAME_PLAYERS_TURN;
        }
        if(StayButton != null)
        {
            StayButton.interactable = CurrentGameState == E_GAME_STATE.GAME_PLAYERS_TURN;
        }
        if (ChangeGameButton != null)
        {
            // Only allow Change Button to show when not in the middle of a game.
            ChangeGameButton.interactable = CurrentGameState == E_GAME_STATE.GAME_IDLE;
        }
    }

    public void ResetTable()
    {
        playerHand.ResetHand();
        dealerHand.ResetHand();

        if (Text_Result != null)
        {
            Text_Result.SetText("");
        }

        if (ReshuffleDeckBetweenHands)
        {
            DeckManager.Instance.ResetDeck();
        }
    }

    public void DealersTurn()
    {
        if(CurrentGameState == E_GAME_STATE.GAME_DEALERS_TURN)
        {
            dealerHand.RevealCards();
            while (dealerHand.HandValue < 17)
            {
                //yield return new WaitForSeconds(2);
                dealerHand.DrawCard();
            }
        }
        EvaluateWin();
    }

    public void EvaluateWin()
    {
        bool playerBlackJack = playerHand.HandValue == 21 && playerHand.CardCount == 2;
        bool dealerBlackJack = dealerHand.HandValue == 21 && dealerHand.CardCount == 2;

        if (playerBlackJack && dealerBlackJack)
        {
            Text_Result.SetText("BlackJack!\nTie!");
        }
        else if(dealerBlackJack)
        {
            Text_Result.SetText("BlackJack!\nDealer Wins");
        }
        else if(playerBlackJack)
        {
            Text_Result.SetText("BlackJack!\nPlayer Wins");
        }
        else if (playerHand.HandValue  > 21)
        {
            // Player busts!
            Text_Result.SetText("PLAYER BUSTS!\nDealer Wins!");
        }
        else if (dealerHand.HandValue > 21)
        {
            // Dealer busts!
            Text_Result.SetText("DEALER BUSTS!\nPlayer Wins");
        }
        else if (dealerHand.HandValue > playerHand.HandValue)
        {
            // Dealer wins.
            Text_Result.SetText("Dealer Wins!");
        }
        else if (playerHand.HandValue > dealerHand.HandValue)
        {
            // Player wins.
            Text_Result.SetText("Player Wins!");
        }
        else
        {
            // It's a push!
            Text_Result.SetText("Tie!");
        }
        SetGameState(E_GAME_STATE.GAME_IDLE);
    }
}
