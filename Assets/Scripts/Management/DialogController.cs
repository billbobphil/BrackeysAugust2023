using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Management
{
    public class DialogController : MonoBehaviour
    {
        [SerializeField] private List<string> dialogLines;
        [SerializeField] private KeyCode keyToAdvanceDialog;
        [SerializeField] private TextMeshProUGUI dialogText;
        private int _currentDialogIndex;
        [SerializeField] private SceneNavigator sceneNavigator;
    
        private void Start()
        {
            _currentDialogIndex = 0;
            dialogText.text = dialogLines[_currentDialogIndex];
        }
    
        private void Update()
        {
            if (!Input.GetKeyDown(keyToAdvanceDialog)) return;
        
            _currentDialogIndex++;
        
            if (_currentDialogIndex < dialogLines.Count)
            {
                dialogText.text = dialogLines[_currentDialogIndex];
            }
            else
            {
                sceneNavigator.GoToGame();
            }
        }
    }
}
