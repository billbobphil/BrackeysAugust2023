using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class SceneNavigator : MonoBehaviour
    {
        public void GoToGame()
        {
            SceneManager.LoadScene("Game");
        }

        public void GoToIntro()
        {
            SceneManager.LoadScene("Intro");
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public void GoToCredits()
        {
            SceneManager.LoadScene("Credits");
        }
    }
}
