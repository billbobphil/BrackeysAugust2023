using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hook
{
    public class PointAtGnomes : MonoBehaviour
    {
        private List<GameObject> _gnomes;
        [SerializeField] private float distanceBetweenHookAndArrows;
        [SerializeField] private GameObject gnomeArrow;
        
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
            gnomeArrow.SetActive(false);
        }

        private void Start()
        {
            _gnomes = GameObject.FindGameObjectsWithTag("Gnome").ToList();
            if (_gnomes.Count == 0)
            {
                gnomeArrow.SetActive(false);
            }
        }

        private void Update()
        {
            if (!gnomeArrow.activeSelf)
            {
                return;
            }
            
            GameObject closestGnome = null;
            float closestDistance = Mathf.Infinity;
            
            foreach (GameObject gnome in _gnomes)
            {
                float distanceBetween = Vector2.Distance(transform.position, gnome.transform.position);
                if (distanceBetween < closestDistance)
                {
                    closestDistance = distanceBetween;
                    closestGnome = gnome;
                }
            }
            
            if (closestGnome is not null)
            {
                GunDirectionHelper.PointGunAtTarget(closestGnome.transform.position, transform.position, gnomeArrow, distanceBetweenHookAndArrows);
            }
        }
    }
}
