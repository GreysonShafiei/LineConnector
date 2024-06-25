using Connect.Common;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Core
{
    public class GameManagement : MonoBehaviour
    {
        public static GameManagement Instance;

        [HideInInspector]
        public int currentStage;

        [HideInInspector]
        public int currentLevel;

        private LevelData currentLevelData;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            currentStage = 1;
            currentLevel = 1;

            levels = new Dictionary<string, LevelData>();

            if (allLevels == null)
            {
                Debug.LogError("allLevels is not assigned in the Inspector");
                return;
            }

            foreach (var item in allLevels.levels)
            {
                levels[item.LevelName] = item;
                Debug.Log($"Added {item.LevelName} to levels dictionary.");
            }

            levels["Level1"] = Level1;
            levels["Level2"] = Level2;
            levels["Level3"] = Level3;
            levels["Level4"] = Level4;
            levels["Level5"] = Level5;
            levels["Level6"] = Level6;
            levels["Level7"] = Level7;
            levels["Level8"] = Level8;
            levels["Level9"] = Level9;
            levels["Level10"] = Level10;

            Debug.Log("Levels Dictionary Initialized:");
            foreach (var level in levels)
            {
                Debug.Log($"{level.Key}: {level.Value}");
            }
        }



        void Start()
        {
            stages = new List<Stage>();

            // Load initial levels from LevelList
            LoadLevels();

            // Generate the initial level
            GenerateAndAddStage();
        }

        private void LoadLevels()
        {
            foreach (var levelData in allLevels.levels)
            {
                string levelName = levelData.LevelName;
                levels[levelName] = levelData;
            }
        }

        private void GenerateAndAddStage()
        {
            string levelName = "Level" + currentStage.ToString() + currentLevel.ToString();
            currentLevelData = getLevel();

            if (currentLevelData == null)
            {
                Debug.LogError($"Level data for {levelName} not found.");
            }
            else
            {
                Debug.Log($"Level data for {levelName} loaded.");
            }
        }


        // Game Variables


        [HideInInspector]
        public string stageName;

        public bool isLevelLocked(int level)
        {
            string levelName = "Level" + currentStage.ToString() + level.ToString();

            if (level == 1)
            {
                PlayerPrefs.SetInt(levelName, 1);
                return true;
            }

            if (PlayerPrefs.HasKey(levelName))
            {
                return PlayerPrefs.GetInt(levelName) == 1;
            }

            PlayerPrefs.SetInt(levelName, 0);
            return false;
        }

        public void unlockedLevel()
        {
            Debug.Log("unlockedLevel called");
            currentLevel++;
            if (currentLevel == 10)
            {
                currentLevel = 1;
                currentStage++;

                if (currentStage == 8)
                {
                    currentStage = 1;
                    goToMenu();
                }
            }

            Debug.Log($"New Level: {currentLevel}, New Stage: {currentStage}");
            string levelName = "Level" + currentStage.ToString() + currentLevel.ToString();
            PlayerPrefs.SetInt(levelName, 1);

            // Generate the next stage when a level is unlocked
            GenerateAndAddStage();
        }
        
        //All Levels List
        [SerializeField]
        private LevelList allLevels;

        // Levels
        [SerializeField]
        private LevelData Level1;
        
        [SerializeField]
        private LevelData Level2;

        [SerializeField]
        private LevelData Level3;

        [SerializeField]
        private LevelData Level4;

        [SerializeField] 
        private LevelData Level5;

        [SerializeField]
        private LevelData Level6;

        [SerializeField]
        private LevelData Level7;

        [SerializeField]
        private LevelData Level8;

        [SerializeField]
        private LevelData Level9;

        [SerializeField]
        private LevelData Level10;

        

        private Dictionary<string, LevelData> levels;

        public LevelData getLevel()
        {
            string levelName = "Level" + currentStage.ToString() + currentLevel.ToString();
            if (levels.ContainsKey(levelName))
            {
                return levels[levelName];
            }

            return Level1;
        }

        public LevelData GetLevelData(string levelName)
        {
            if (levels.ContainsKey(levelName))
            {
                return levels[levelName];
            }
            return null;
        }


        // Scene Management
        private const string MainMenu = "MainMenu";
        private const string GamePlay = "GamePlay";

        public void goToMenu()
        {
            Debug.Log("goToMenu called");
            UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu);
            Awake();
        }

        public void goToGamePlay()
        {
            Debug.Log("goToGamePlay called");
            string levelSceneName = "Level" + currentStage.ToString();
            Debug.Log($"Loading Scene: {levelSceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneName);
        }


        // Stages
        public List<Stage> stages;
    }
}
