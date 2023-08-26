using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> fishPrefabs;
        [SerializeField] private List<GameObject> gnomePrefabs;
        public int numberOfGnomesToCreate;
        [SerializeField] private int numberOfInitialLevelsToSkip;
        [SerializeField] private int numberOfLevelsPerCycle;
        [SerializeField] private int chanceToSpawnGnome;
        [SerializeField] private int chanceToSpawnFish;
        [SerializeField] private int chanceToSpawnBubble;
        [SerializeField] private GameObject oceanFloorPrefab;
        [SerializeField] private int distanceToLeaveToOceanFloor;
        [SerializeField] private GameObject bubblePrefab;
        private int _currentYLevel = 2;
        
        private Dictionary<int, GameObject> _levelObjects = new Dictionary<int, GameObject>();

        private void Start()
        {
            GenerateLevel();
        }

        private void GenerateLevel()
        {
            //skip some amount of initial levels
            _currentYLevel += numberOfInitialLevelsToSkip;
            
            for(int i = 0; i < numberOfGnomesToCreate; i++)
            {
                bool lastPass = i == numberOfGnomesToCreate - 1;
                RunCycle(lastPass);
            }

            _currentYLevel += distanceToLeaveToOceanFloor;
            float x = Random.Range(-FishingBounds.HorizontalBoundingDistance, FishingBounds.HorizontalBoundingDistance);
            GameObject createdGnome = Instantiate(gnomePrefabs[Random.Range(0, gnomePrefabs.Count)], new Vector3(x, -(_currentYLevel - 2), 0), Quaternion.identity);
            _levelObjects.Add(_currentYLevel, createdGnome);
            Instantiate(oceanFloorPrefab, new Vector3(0, -_currentYLevel, 0), Quaternion.identity);
        }

        private void RunCycle(bool lastPass)
        {
            bool gnomeSpawned = false;
            
            for(int i = _currentYLevel; i < _currentYLevel + numberOfLevelsPerCycle; i += 2)
            {
                //10% chance to spawn a gnome
                if (!gnomeSpawned && !lastPass && Random.Range(0, 100) < chanceToSpawnGnome)
                {
                    float x = Random.Range(-FishingBounds.HorizontalBoundingDistance, FishingBounds.HorizontalBoundingDistance);
                    GameObject createdGnome = Instantiate(gnomePrefabs[Random.Range(0, gnomePrefabs.Count)], new Vector3(x, -i, 0), Quaternion.identity);
                    gnomeSpawned = true;
                    _levelObjects.Add(i, createdGnome);
                }
                else if (Random.Range(0, 100) < chanceToSpawnFish)
                {
                    float x = Random.Range(-FishingBounds.HorizontalBoundingDistance, FishingBounds.HorizontalBoundingDistance);
                    
                    GameObject createdFish = Instantiate(fishPrefabs[Random.Range(0, fishPrefabs.Count)], new Vector3(x, -i, 0), Quaternion.identity);
                    _levelObjects.Add(i, createdFish);
                }
                else if (Random.Range(0, 100) < chanceToSpawnBubble)
                {
                    float x = Random.Range(-FishingBounds.HorizontalBoundingDistance, FishingBounds.HorizontalBoundingDistance);
                    GameObject createdBubble = Instantiate(bubblePrefab, new Vector3(x, -i, 0), Quaternion.identity);
                    _levelObjects.Add(i, createdBubble);
                }
            }

            if (!gnomeSpawned)
            {
                if (_levelObjects.TryGetValue(_currentYLevel + numberOfLevelsPerCycle, out GameObject fish))
                {
                    Destroy(fish);
                    _levelObjects.Remove(_currentYLevel + numberOfLevelsPerCycle);
                }
                
                GameObject gnome = Instantiate(gnomePrefabs[Random.Range(0, gnomePrefabs.Count)], new Vector3(0, -(_currentYLevel + numberOfLevelsPerCycle), 0),
                    Quaternion.identity);
                _levelObjects.Add(_currentYLevel + numberOfLevelsPerCycle, gnome);
            }
            
            _currentYLevel += numberOfLevelsPerCycle + 2;
        }
    }
}
