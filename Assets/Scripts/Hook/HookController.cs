using System;
using Management;
using UnityEngine;
using UnityEngine.Events;

namespace Hook
{
    public class HookController : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = .5f;
        [SerializeField] private float fastSpeed = 1f;
        [SerializeField] private Rigidbody2D hookRigidbody;
        [SerializeField] private Vector2 currentDirection;
        private IHookable _hookedObject;
        [SerializeField] private float horizontalSpeed = .1f;

        public static UnityAction<IHookable> OnCaughtSomething;

        private void FixedUpdate()
        {
            hookRigidbody.velocity = currentDirection * baseSpeed;
            
            if (currentDirection == Vector2.down && Input.GetKey(KeyCode.S))
            {
                MoveFast(Vector2.down);
            }

            if (currentDirection == Vector2.up && Input.GetKey(KeyCode.W))
            {
                MoveFast(Vector2.up);
            }

            //TODO: when hook at max length in one direction should have some sort of indication that it can't go further
            if (Input.GetKey(KeyCode.A))
            {
                if (transform.position.x >= -FishingBounds.HorizontalBoundingDistance)
                {
                    transform.position += (Vector3)Vector2.left * horizontalSpeed;
                }
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                if (transform.position.x <= FishingBounds.HorizontalBoundingDistance)
                {
                    transform.position += (Vector3)Vector2.right * horizontalSpeed;    
                }
            }
        }

        private void MoveFast(Vector2 direction)
        {
            hookRigidbody.velocity = direction * fastSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("CaughtPlane"))
            {
                if (_hookedObject is not null)
                {
                    OnCaughtSomething?.Invoke(_hookedObject);
                    Destroy(gameObject);
                }
                return;
            }
            
            if (_hookedObject is not null)
            {
                return;
            }
            
            IHookable hookableObject = other.gameObject.GetComponent<IHookable>();
            
            if (hookableObject is not null)
            {
                hookableObject.OnHooked(gameObject);
                OnSomethingHooked(hookableObject);
            }
        }

        private void OnSomethingHooked(IHookable hookedObject)
        {
            currentDirection = Vector2.up;
            _hookedObject = hookedObject;
        }
    }
}