using Cinemachine;
using UnityEngine;

namespace Hook
{
    public class HookSpawner : MonoBehaviour
    {
        private bool _doesHookExist;
        [SerializeField] private GameObject hookPrefab;
        [SerializeField] private Transform hookSpawnPoint;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        private void OnEnable()
        {
            HookController.OnSurfaced += OnSurfaced;
        }

        private void OnDisable()
        {
            HookController.OnSurfaced -= OnSurfaced;
        }
        
        private void Update()
        {
            if (_doesHookExist)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _doesHookExist = true;
                GameObject createdHook = Instantiate(hookPrefab, hookSpawnPoint.position, new Quaternion());
                cinemachineVirtualCamera.Follow = createdHook.transform;
            }
        }

        private void OnSurfaced()
        {
            _doesHookExist = false;
        }
    }
}