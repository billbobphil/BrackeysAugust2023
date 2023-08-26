using System;
using Management;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace Hook
{
    public class HookController : MonoBehaviour
    {
        [SerializeField] private float baseSpeed;
        [SerializeField] private float fastSpeed;
        [SerializeField] private float gameWinSpeedIncrement;
        [SerializeField] private Rigidbody2D hookRigidbody;
        [SerializeField] private Vector2 currentDirection;
        private IHookable _hookedObject;
        [SerializeField] private float horizontalSpeed = .1f;
        [SerializeField] private HookArrowController hookArrowController;
        private LineRenderer _lineRenderer;
        private SoundEffectManager _soundEffectManager;
        [SerializeField] private MMFeedbacks collisionFeedback;

        public static UnityAction<IHookable> OnCaughtSomething;
        public static UnityAction<IHookable> OnHookedSomething;
        public static UnityAction OnSurfaced;

        private void OnEnable()
        {
            HookArrowController.ArrowGameWon += OnArrowGameWon;
            HookArrowController.ArrowGameLostTooManyTimes += OnArrowGameLostTooManyTimes;
        }
        
        private void OnDisable()
        {
            HookArrowController.ArrowGameWon -= OnArrowGameWon;
            HookArrowController.ArrowGameLostTooManyTimes -= OnArrowGameLostTooManyTimes;
        }

        private void Start()
        {
            _soundEffectManager = GameObject.FindWithTag("SoundEffectManager").GetComponent<SoundEffectManager>();
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, transform.position);
            Vector3 rodPosition = GameObject.FindWithTag("FishingRod").transform.position;
            _lineRenderer.SetPosition(1, rodPosition);
            _lineRenderer.startWidth = .05f;
            _lineRenderer.endWidth = .05f;
            _soundEffectManager.reelingSoundEffect.Stop();
        }

        private void OnArrowGameWon()
        {
            baseSpeed += gameWinSpeedIncrement;
        }
        
        private void OnArrowGameLostTooManyTimes()
        {
            if (_hookedObject is Gnome.Gnome gnome)
            {
                _hookedObject = null;
                gnome.Drop();
                _soundEffectManager.gnomeDrop.Play();
            }
        }

        private void FixedUpdate()
        {
            if (currentDirection == Vector2.up && !_soundEffectManager.reelingSoundEffect.isPlaying)
            {
                _soundEffectManager.reelingSoundEffect.Play();
            }
            
            hookRigidbody.velocity = currentDirection * baseSpeed;
            
            if (currentDirection == Vector2.down && Input.GetKey(KeyCode.S))
            {
                MoveFast(Vector2.down);
            }

            if (currentDirection == Vector2.down && Input.GetKey(KeyCode.R))
            {
                GetComponent<SpriteRenderer>().flipY = false;
                currentDirection = Vector2.up;
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

        private void LateUpdate()
        {
            _lineRenderer.SetPosition(0, transform.position);
        }

        private void MoveFast(Vector2 direction)
        {
            hookRigidbody.velocity = direction * fastSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("CaughtPlane"))
            {
                _soundEffectManager.reelingSoundEffect.Stop();
                
                if (_hookedObject is not null)
                {
                    OnCaughtSomething?.Invoke(_hookedObject);
                    
                    if(_hookedObject is Gnome.Gnome)
                    {
                        _soundEffectManager.caughtGnome.Play();
                    }
                    else if (_hookedObject is Fish.Fish)
                    {
                        _soundEffectManager.caughtFish.Play();
                    }
                }

                if (currentDirection == Vector2.up)
                {
                    OnSurfaced?.Invoke();
                    Destroy(gameObject);    
                }
                
                return;
            }

            if (other.CompareTag("OceanFloor"))
            {
                currentDirection = Vector2.up;
            }
            
            if(other.CompareTag("Fish") && _hookedObject is Gnome.Gnome)
            {
                collisionFeedback.PlayFeedbacks();
                _soundEffectManager.hitObstacle.Play();
                hookArrowController.FailArrowGame();
                hookArrowController.ResetGame();
                return;
            }
            
            //Guard so we can't catch two things
            if (_hookedObject is not null)
            {
                return;
            }
            
            IHookable hookableObject = other.gameObject.GetComponent<IHookable>();
            
            if (hookableObject is not null)
            {
                GetComponent<SpriteRenderer>().flipY = false;
                hookableObject.OnHooked(gameObject);
                OnSomethingHooked(hookableObject);
                OnHookedSomething?.Invoke(hookableObject);
                
                if(hookableObject is Gnome.Gnome)
                {
                    _soundEffectManager.hookedGnome.Play();
                }
                else if (hookableObject is Fish.Fish)
                {
                    _soundEffectManager.hookedFish.Play();
                }
            }
        }

        private void OnSomethingHooked(IHookable hookedObject)
        {
            currentDirection = Vector2.up;
            _hookedObject = hookedObject;
        }
    }
}