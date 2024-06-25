using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Connect.Core
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu Instance;

        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject stagePanel;
        [SerializeField] private GameObject levelPanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("MainMenu instance set in Awake");
            }
            

            Instance = this;
            titlePanel.SetActive(true);
            stagePanel.SetActive(false);
            levelPanel.SetActive(false);
        }

        public void clickedPlay()
        {
            titlePanel.SetActive(false);
            stagePanel.SetActive(true);
        }

        public void clickedBack()
        {
            titlePanel.SetActive(true);
            stagePanel.SetActive(false);
        }

        public void clickedToTitle()
        {
            titlePanel.SetActive(true);
            stagePanel.SetActive(false);
        }

        public void clickedBackToStage()
        {
            stagePanel.SetActive(true);
            levelPanel.SetActive(false);
        }

        public UnityAction LevelOpened;

        [HideInInspector]
        public Color currentColor;

        [SerializeField]
        private TextMeshProUGUI levelTitleText;
        [SerializeField]
        private Image levelTitleImage;

        public void clickedStage(string stageName, Color stageColor)
        {
            stagePanel.SetActive(false);
            levelPanel.SetActive(true);
            currentColor = stageColor;
            levelTitleText.text = stageName;
            levelTitleImage.color = currentColor;
            LevelOpened?.Invoke();
        }
    }
}