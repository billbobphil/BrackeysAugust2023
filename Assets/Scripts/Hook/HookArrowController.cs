using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Management;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Hook
{
    public class HookArrowController : MonoBehaviour
    {
        [SerializeField] private float timeBetweenMiniGameSpawnsMin = 1.5f;
        [SerializeField] private float timeBetweenMiniGameSpawnsMax = 4f;
        private List<(ArrowDirections direction, GameObject arrow)> _sequence;
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private List<GameObject> arrowSpawnPoints;
        private List<GameObject> _spawnedArrows = new List<GameObject>();
        [SerializeField] private float timeToFailure = 3f;
        private SoundEffectManager _soundEffectManager;

        private Color beige = new Color(209/255f, 191/255f, 176/255f);
        private Color red = new Color(187/255f, 70/255f, 79/255f);
        private Color blue = new Color(122 / 255f, 156 / 255f, 150 / 255f);
        
        private float _timeBetweenGames;
        private bool _isGameEnabled;
        private bool _areArrowsCurrentlySpawned;
        private float _timer = 0f;
        private int _currentArrowIndex;
        private bool _allowInput;
        private float _failureTimer;
        private int _gamesFailed;

        public static UnityAction ArrowGameWon;
        public static UnityAction<int> ArrowGameLost;
        public static UnityAction ArrowGameLostTooManyTimes;

        private enum ArrowDirections
        {
            Up,
            Down,
            Left,
            Right
        }

        private readonly Dictionary<ArrowDirections, KeyCode[]> _arrowKeyCodes = new()
        {
            {
                ArrowDirections.Up, new [] { KeyCode.UpArrow, KeyCode.UpArrow }
            },
            {
                ArrowDirections.Down, new [] { KeyCode.DownArrow, KeyCode.DownArrow }
            },
            {
                ArrowDirections.Left, new [] { KeyCode.LeftArrow, KeyCode.LeftArrow }
            },
            {
                ArrowDirections.Right, new [] { KeyCode.RightArrow, KeyCode.RightArrow }
            }
        };
        
        private void Start()
        {
            _soundEffectManager = GameObject.FindWithTag("SoundEffectManager").GetComponent<SoundEffectManager>();
            _timeBetweenGames = Random.Range(timeBetweenMiniGameSpawnsMin, timeBetweenMiniGameSpawnsMax);
        }
        
        private void OnEnable()
        {
            HookController.OnHookedSomething += OnHookedSomething;   
        }
        
        private void OnDisable()
        {
            HookController.OnHookedSomething -= OnHookedSomething;
        }
        
        private void OnHookedSomething(IHookable hookedObject)
        {
            if (hookedObject is Gnome.Gnome gnome)
            {
                _isGameEnabled = true;
                _areArrowsCurrentlySpawned = false;    
            }
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            
            if(_areArrowsCurrentlySpawned)
            {
                _failureTimer += Time.deltaTime;

                if (_failureTimer >= timeToFailure)
                {
                    FailArrowGame();
                    ResetGame();
                    return;
                }
                
                if (!Input.anyKeyDown || !_allowInput) return;
                KeyCode pressedKey = KeyCode.None;
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        pressedKey = keyCode;
                    }
                }

                if (pressedKey is KeyCode.UpArrow or KeyCode.DownArrow or KeyCode.LeftArrow or KeyCode.RightArrow)
                {
                    KeyCode[] requiredKeyCodes = _arrowKeyCodes[_sequence[_currentArrowIndex].direction];
                    bool correctInput = requiredKeyCodes.Any(keyCode => keyCode == pressedKey);
                    if (correctInput)
                    {
                        ProcessArrowSuccess();
                    }
                    else
                    {
                        StartCoroutine(ProcessArrowFailure());
                    }   
                }
                
                return;
            }
            
            if (_timer >= _timeBetweenGames && _isGameEnabled && !_areArrowsCurrentlySpawned)
            {
                DeployArrows();
                _timer = 0f;
            }
        }

        private void DeployArrows()
        {
            _areArrowsCurrentlySpawned = true;
            _allowInput = true;
                
            int sequenceLength = arrowSpawnPoints.Count;
            _sequence = new List<(ArrowDirections direction, GameObject arrow)>();

            for (int i = 0; i < sequenceLength; i++)
            {
                int randomDirection = Random.Range(0, 4);
                ArrowDirections direction = (ArrowDirections) randomDirection;
                
                GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoints[i].transform);
                _spawnedArrows.Add(arrow);
                
                float rotation = direction switch
                {
                    ArrowDirections.Up => 0,
                    ArrowDirections.Down => 180,
                    ArrowDirections.Left => 90,
                    ArrowDirections.Right => 270,
                    _ => 0
                };

                arrow.transform.localPosition = Vector3.zero;
                arrow.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                _sequence.Add((direction, arrow));
            }
        }
        
        private void ProcessArrowSuccess()
        {
            _sequence[_currentArrowIndex].arrow.GetComponentInChildren<SpriteRenderer>().color = blue;
            _soundEffectManager.successArrowSoundEffect.Play();
            
            if (_sequence.Count - 1 != _currentArrowIndex)
            {
                _currentArrowIndex++;
            }
            else
            {
                ArrowGameWon?.Invoke();
                ResetGame();
                _failureTimer = 0;
            }
        }
        
        private IEnumerator ProcessArrowFailure()
        {
            FailArrowGame();
            _allowInput = false;
            _soundEffectManager.failureArrowSoundEffect.Play();
            
            for (int i = 0; i <= _currentArrowIndex; i++)
            {
                if (_sequence[i].arrow is not null)
                {
                    _sequence[i].arrow.GetComponentInChildren<SpriteRenderer>().color = red;    
                }
            }

            yield return new WaitForSecondsRealtime(1);
            
            for (int i = 0; i <= _currentArrowIndex; i++)
            {
                try
                {
                    _sequence[i].arrow.GetComponentInChildren<SpriteRenderer>().color = beige;   
                }
                catch (Exception)
                {
                    //squash
                }
            }

            _currentArrowIndex = 0;
            _allowInput = true;
            ResetGame();
        }

        public void FailArrowGame()
        {
            _failureTimer = 0;
            _gamesFailed++;
            
            Debug.Log($"Failed {_gamesFailed} times");

            if (_gamesFailed >= 3)
            {
                ArrowGameLostTooManyTimes?.Invoke();
                _isGameEnabled = false;
            }
            else
            {
                ArrowGameLost?.Invoke(_gamesFailed);    
            }
        }

        public void ResetGame()
        {
            _areArrowsCurrentlySpawned = false;
            _currentArrowIndex = 0;
            _spawnedArrows.ForEach(Destroy);
        }
    }
}