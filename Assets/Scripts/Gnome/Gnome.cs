using Hook;
using UnityEngine;

namespace Gnome
{
    public class Gnome : MonoBehaviour, IHookable
    {
        private bool _hooked;
        private Vector2 _startingPosition;

        private void Start()
        {
            _startingPosition = transform.position;
        }

        public void Drop()
        {
            transform.parent = null;
            transform.position = _startingPosition;
        }
        
        public void OnHooked(GameObject hook)
        {
            transform.parent = hook.transform;
            _hooked = true;
        }
    }
}