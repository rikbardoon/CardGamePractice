using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackJackManager : MonoBehaviour
{
    public const int BLACK_JACK_LIMIT = 21;

    private enum GameState
    {
        GAME_IDLE = 0,
        GAME_PLAYERS_TURN,
        GAME_DEALERS_TURN,
        STATE_COUNT
    }

    public BlackJackHand playerHand;
    public BlackJackHand dealerHand;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(playerHand != null, "Missing player hand object!");
        Debug.Assert(dealerHand != null, "Missing dealer hand object!");
    }

    public void ChangeGamePressed()
    {
        // Change to main screen for game selection.
        UnityEngine.Debug.Log("Change Game button pressed");
    }

    public void StartGamePressed()
    {
        playerHand.DrawCard(false);
        playerHand.DrawCard(false);

        dealerHand.DrawCard(true);
        dealerHand.DrawCard(false);
    }

    public void HitButtonPressed()
    {
        playerHand.DrawCard();
        if(playerHand.HandValue > 21)
        {
            // Player busts!
            // Reveal dealer's hand?
            // Go to end game.
        }
    }

    public void StayButtonPressed()
    {
        DealersTurn();
    }

    public void DealersTurn()
    {
        dealerHand.RevealCards();
        while (dealerHand.HandValue >= 17)
        {
            //yield return new WaitForSeconds(2);
            dealerHand.DrawCard();
        }
    }

    public void EvaluateWin()
    {
        if(playerHand.HandValue  > 21)
        {
            // Player busts!
            // go to end game.
        }
        if (dealerHand.HandValue > 21)
        {
            // Dealer busts!
            // go to end game.
        }
        else if (dealerHand.HandValue > playerHand.HandValue)
        {
            // Dealer wins.
            // go to end game.
        }
        else if (playerHand.HandValue > dealerHand.HandValue)
        {
            // Player wins.
            // go to end game.
        }
        else
        {
            // It's a push!
            // go to end game.
        }
    }
}
