using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField] private List<string> dialogLines;
    [SerializeField] private KeyCode keyToAdvanceDialog;
    [SerializeField] private TextMeshProUGUI dialogText;
    private int _currentDialogIndex;
    
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
            Debug.Log("Dialog complete");
            //TODO: trigger scene advance
        }
    }
}
