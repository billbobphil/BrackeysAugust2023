using UnityEngine;
using Utilities;

namespace Management
{
    public class TimerController : MonoBehaviour
    {
        [SerializeField] private Timer timer;

        private void Start()
        {
            timer.StartTimer();
        }
    }
}
