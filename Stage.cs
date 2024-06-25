using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connect.Core;

namespace Connect.Core
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] public string stageName;
        [SerializeField] public Color stageColor;
        [SerializeField] public int stageNumber;
        [SerializeField] public Button button;

        private void Awake()
        {
            button.onClick.AddListener(clickedButton);
        }

        private void clickedButton()
        {
            GameManagement.Instance.currentStage = stageNumber;
            GameManagement.Instance.stageName = stageName;
            MainMenu.Instance.clickedStage(stageName, stageColor);
        }
    }
}