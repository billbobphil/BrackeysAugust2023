using System;
using Hook;
using Management;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fish
{
    public class Fish : MonoBehaviour, IHookable
    {
        [SerializeField] private float speedRangeBase = .1f;
        [SerializeField] private float speedRangeMax = .6f;
        [SerializeField] private float swimDistanceFromOriginMin = 2f;
        [SerializeField] private float swimDistanceFromOriginMax = 5f;
        private Vector2 _startingPosition;
        private Vector2 _horizontalDirection;
        private float _speed;
        private float _swimDistanceFromOrigin;
        private bool _hooked;
        
        private void Start()
        {
            _speed = Random.Range(speedRangeBase, speedRangeMax);
            _swimDistanceFromOrigin = Random.Range(swimDistanceFromOriginMin, swimDistanceFromOriginMax);
            _horizontalDirection = Random.Range(0, 2) == 0 ? Vector2.left : Vector2.right;
            _startingPosition = transform.position;
            PickCorrectSpriteDirection();
        }

        private void FixedUpdate()
        {
            Swim();
        }

        private void Swim()
        {
            if (_hooked)
                return;
            transform.position += (Vector3)_horizontalDirection * _speed;
            if (Vector2.Distance(transform.position, _startingPosition) >= _swimDistanceFromOrigin ||
                transform.position.x <= -FishingBounds.HorizontalBoundingDistance ||
                transform.position.x >= FishingBounds.HorizontalBoundingDistance)
            {
                _horizontalDirection *= -1;
                PickCorrectSpriteDirection();
            }
        }

        public void OnHooked(GameObject hook)
        {
            transform.parent = hook.transform;
            _hooked = true;
        }

        private void PickCorrectSpriteDirection()
        {
            if(_horizontalDirection == Vector2.left)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
    }
}