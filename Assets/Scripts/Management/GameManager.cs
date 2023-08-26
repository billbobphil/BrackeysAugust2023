using System.Collections.Generic;
using Hook;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Management
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelGenerator levelGenerator;
        private int _gnomesCaught;
        [SerializeField] private List<Image> gnomeImages = new List<Image>();
        [SerializeField] private Sprite caughtGnomeSprite;
        [SerializeField] private ScoreScreen scoreScreen;
        [SerializeField] private Timer timer;

        private void OnEnable()
        {
            HookController.OnCaughtSomething += OnCaughtSomething;
            Timer.TimerExpired += OnTimerExpired;
        }
        
        private void OnDisable()
        {
            HookController.OnCaughtSomething -= OnCaughtSomething;
            Timer.TimerExpired -= OnTimerExpired;
        }

        private void Awake()
        {
            Time.timeScale = 1;
        }
        
        private void OnCaughtSomething(IHookable hookable)
        {
            if (hookable is Gnome.Gnome)
            {
                gnomeImages[_gnomesCaught].sprite = caughtGnomeSprite;
                _gnomesCaught++;
            }
            
            if(_gnomesCaught >= levelGenerator.numberOfGnomesToCreate)
            {
                Debug.Log("You win!");
                scoreScreen.ShowScoreScreen(_gnomesCaught, (int)timer.GetTime(), true);
            }
        }

        private void OnTimerExpired()
        {
            Debug.Log("You lose!");
            scoreScreen.ShowScoreScreen(_gnomesCaught, (int)timer.GetTime(), false);
        }
    }
}
