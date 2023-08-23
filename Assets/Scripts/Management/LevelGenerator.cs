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
        [SerializeField] private GameObject oceanFloorPrefab;
        [SerializeField] private int distanceToLeaveToOceanFloor;
        private int currentYLevel = 2;
        
        private Dictionary<int, GameObject> _levelObjects = new Dictionary<int, GameObject>();

        private void Start()
        {
            GenerateLevel();
        }

        private void GenerateLevel()
        {
            //skip some amount of initial levels
            currentYLevel += numberOfInitialLevelsToSkip;
            
            for(int i = 0; i < numberOfGnomesToCreate; i++)
            {
               RunCycle();
            }

            currentYLevel += distanceToLeaveToOceanFloor;
            Instantiate(oceanFloorPrefab, new Vector3(0, -currentYLevel, 0), Quaternion.identity);
        }

        private void RunCycle()
        {
            bool gnomeSpawned = false;
            
            for(int i = currentYLevel; i < currentYLevel + numberOfLevelsPerCycle; i += 2)
            {
                //10% chance to spawn a gnome
                if (!gnomeSpawned && Random.Range(0, 100) < chanceToSpawnGnome)
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
            }

            if (!gnomeSpawned)
            {
                if (_levelObjects.TryGetValue(currentYLevel + numberOfLevelsPerCycle, out GameObject fish))
                {
                    Destroy(fish);
                    _levelObjects.Remove(currentYLevel + numberOfLevelsPerCycle);
                }
                
                GameObject gnome = Instantiate(gnomePrefabs[Random.Range(0, gnomePrefabs.Count)], new Vector3(0, -(currentYLevel + numberOfLevelsPerCycle), 0),
                    Quaternion.identity);
                _levelObjects.Add(currentYLevel + numberOfLevelsPerCycle, gnome);
            }
            
            currentYLevel += numberOfLevelsPerCycle + 2;
        }
    }
}
