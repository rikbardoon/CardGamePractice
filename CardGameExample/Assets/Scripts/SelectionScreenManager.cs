using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreenManager : MonoBehaviour
{
    public void PokerSelected()
    {
        SceneManager.LoadScene("PokerScreen");
    }

    public void BlackJackSelected()
    {
        SceneManager.LoadScene("BlackJackScreen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
