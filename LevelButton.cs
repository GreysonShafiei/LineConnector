using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Connect.Core;

namespace Connect.Core
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] private Color inActiveColor;

        private bool isLevelLocked;
        private int currentLevel;

        private void Awake()
        {
            Debug.Log("LevelButton Awake called");
        }

        private void OnEnable()
        {
            Debug.Log("LevelButton OnEnable called");
            StartCoroutine(WaitForMainMenu());
        }

        private IEnumerator WaitForMainMenu()
        {
            while (MainMenu.Instance == null)
            {
                yield return null;
            }

            MainMenu.Instance.LevelOpened += LevelOpened;
        }

        private void OnDisable()
        {
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.LevelOpened -= LevelOpened;
            }
        }

        private void LevelOpened()
        {
            string gameObjectName = gameObject.name;
            string[] parts = gameObjectName.Split('_');
            levelText.text = parts[parts.Length - 1];
            Debug.Log($"Level text to parse: {levelText.text}");

            if (int.TryParse(levelText.text, out currentLevel))
            {
                isLevelLocked = GameManagement.Instance.isLevelLocked(currentLevel);
                image.color = isLevelLocked ? MainMenu.Instance.currentColor : inActiveColor;
            }
            else
            {
                Debug.LogError($"Invalid level text: {levelText.text}");
            }
        }

        private void clicked()
        {
            if (!isLevelLocked)
            {
                return;
            }

            GameManagement.Instance.currentLevel = currentLevel;
            GameManagement.Instance.goToGamePlay();
        }

    }
}