using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Management
{
    public class ScoreScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI minutesText;
        [SerializeField] private TextMeshProUGUI secondsText;
        [SerializeField] private List<Image> caughtGnomeImages;
        [SerializeField] private Sprite caughtGnomeSprite;
        [SerializeField] private GameObject scoreScreenCanvas;
        private readonly Color _redColor = new(187/255f, 70/255f, 79/255f);
        private SoundEffectManager _soundEffectManager;

        private void Start()
        {
            scoreScreenCanvas.SetActive(false);
            _soundEffectManager = GameObject.FindWithTag("SoundEffectManager").GetComponent<SoundEffectManager>();
        }
        
        public void ShowScoreScreen(int numberOfGnomesCaught, int timeRemaining, bool isVictory)
        {
            if(isVictory)
                _soundEffectManager.victory.Play();
            else
                _soundEffectManager.defeat.Play();
            
            Time.timeScale = 0;
            for (int i = 0; i < numberOfGnomesCaught; i++)
            {
                caughtGnomeImages[i].sprite = caughtGnomeSprite;
            }

            if (timeRemaining <= 0)
            {
                minutesText.color = _redColor;
                secondsText.color = _redColor;
            }
            
            minutesText.text = (timeRemaining / 60).ToString("00");
            secondsText.text = (timeRemaining % 60).ToString("00");
            scoreScreenCanvas.SetActive(true);
        }
    }
}
