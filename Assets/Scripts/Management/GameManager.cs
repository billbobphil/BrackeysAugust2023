using Hook;
using UnityEngine;
using Utilities;

namespace Management
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelGenerator levelGenerator;
        private int _gnomesCaught;

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
        
        private void OnCaughtSomething(IHookable hookable)
        {
            if (hookable is Gnome.Gnome)
            {
                _gnomesCaught++;
            }
            
            if(_gnomesCaught >= levelGenerator.numberOfGnomesToCreate)
            {
                Debug.Log("You win!");
                //TODO: victory things
            }
        }

        private void OnTimerExpired()
        {
            Debug.Log("You lose!");
            //TODO: loss things
        }
    }
}
