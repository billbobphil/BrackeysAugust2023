using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class SceneNavigator : MonoBehaviour
    {
        [SerializeField] private Animator transitionAnimator;
        [SerializeField] private float transitionTime = 1f;
        
        public void GoToGame()
        {
            StartCoroutine(TransitionLevel("Game"));
        }

        public void GoToIntro()
        {
            StartCoroutine(TransitionLevel("Intro"));
        }

        public void GoToMenu()
        {
            StartCoroutine(TransitionLevel("Menu"));
        }

        public void GoToCredits()
        {
            StartCoroutine(TransitionLevel("Credits"));
        }
        
        IEnumerator TransitionLevel(string sceneName)
        {
            transitionAnimator.SetBool("Open", false);
            yield return new WaitForSeconds(transitionTime);
            SceneManager.LoadScene(sceneName);

        }
    }
}
