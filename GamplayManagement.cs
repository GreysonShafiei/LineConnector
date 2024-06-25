using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Connect.Common;

namespace Connect.Core
{
    public class GameplayManagement : MonoBehaviour
    {
        public static GameplayManagement Instance;

        [HideInInspector] public bool hasGameFinished;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private GameObject winText;
        [SerializeField] private SpriteRenderer clickHighlight;
        [HideInInspector] private int LevelNumberCount;

        private LevelData currentLevelData;

        private void Awake()
        {
            Instance = this;
            StartCoroutine(InitializeAfterDelay());
        }

        private IEnumerator InitializeAfterDelay()
        {
            while (GameManagement.Instance == null)
            {
                yield return null;
            }

            Debug.Log("GameManagement.Instance is now set");
            InitializeGameplay();
        }

        private void InitializeGameplay()
        {
            Debug.Log("InitializeGameplay called");
            if (GameManagement.Instance == null)
            {
                Debug.LogError("GameManagement.Instance is null in GameplayManagement.InitializeGameplay");
                return;
            }

            hasGameFinished = false;
            winText.SetActive(false);
            titleText.gameObject.SetActive(true);
            LevelNumberCount = functionCallCounter + 1;
            titleText.text = GameManagement.Instance.stageName + " - " + LevelNumberCount;
            currentLevelData = GameManagement.Instance.getLevel();
            if (currentLevelData == null)
            {
                Debug.LogError("currentLevelData is null in GameplayManagement.InitializeGameplay");
                return;
            }

            Debug.Log("Spawning Board");
            SpawnBoard();
            Debug.Log("Spawning Nodes");
            SpawnNodes();
        }
        


        [SerializeField] private SpriteRenderer boardPrefab, CellPrefab;

        private void SpawnBoard()
        {
            int currentLevelSize = GameManagement.Instance.currentStage + 4;

            var board = Instantiate(boardPrefab,
                new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
                Quaternion.identity);

            board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(CellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity);
                }
            }

            Camera.main.orthographicSize = currentLevelSize + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);

            clickHighlight.size = new Vector2(currentLevelSize / 4f, currentLevelSize / 4f);
            clickHighlight.transform.position = Vector3.zero;
            clickHighlight.gameObject.SetActive(false);
        }


        //private LevelData currentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;

        public Dictionary<Vector2Int, Node> _nodeGrid;

        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();

            int currentLevelSize = GameManagement.Instance.currentStage + 4;
            Node spawnedNode;
            Vector3 spawnPos;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i, j);

                    if (colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                    }

                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);

                }
            }

            List<Vector2Int> offsetPos = new List<Vector2Int>()
            {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right };

            foreach (var item in _nodeGrid)
            {
                foreach (var offset in offsetPos)
                {
                    var checkPos = item.Key + offset;
                    if (_nodeGrid.ContainsKey(checkPos))
                    {
                        item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                    }
                }
            }


        }

        public List<Color> NodeColors;

        public int GetColorId(int i, int j)
        {
            if (currentLevelData == null)
            {
                Debug.LogError("currentLevelData is not assigned");
                return -1;
            }

            List<Edge> edges = currentLevelData.Edges;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].Start == point ||
                    edges[colorId].End == point)
                {
                    return colorId;
                }
            }

            return -1;
        }


        public Color GetHighLightColor(int colorID)
        {
            Color result = NodeColors[colorID % NodeColors.Count];
            result.a = 0.4f;
            return result;
        }


        private Node startNode;

        private void Update()
        {
            if (hasGameFinished) return;

            if (Input.GetMouseButtonDown(0))
            {
                startNode = null;
                return;
            }

            if (Input.GetMouseButton(0))
            {

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (startNode == null)
                {
                    if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode)
                        && tNode.IsClickable)
                    {
                        startNode = tNode;
                        clickHighlight.gameObject.SetActive(true);
                        clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;
                        clickHighlight.color = GetHighLightColor(tNode.colorId);
                    }

                    return;
                }

                clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;

                if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode)
                    && startNode != tempNode)
                {
                    if (startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                    {
                        return;
                    }

                    startNode.UpdateInput(tempNode);
                    CheckWin();
                    startNode = null;
                }

                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                startNode = null;
                clickHighlight.gameObject.SetActive(false);
            }

        }



        private void CheckWin()
        {
            bool IsWinning = true;

            foreach (var item in _nodes)
            {
                item.SolveHighlight();
            }

            foreach (var item in _nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning)
                {
                    return;
                }
            }

            GameManagement.Instance.unlockedLevel();

            winText.gameObject.SetActive(true);
            clickHighlight.gameObject.SetActive(false);

            hasGameFinished = true;
        }


        
        public void clickedBack()
        {
            GameManagement.Instance.goToMenu();
        }

        //variable tracking the level
        private int levelTrack = 0;
        public void clickedRestart()
        {
            hasGameFinished = false;
            winText.SetActive(false);
            if (levelTrack == 0)
            {
                GameManagement.Instance.goToGamePlay();
            }
            else if (levelTrack == 1)
            {
                LevelNumberCount = 2;
                InitLevel(level2Data);
            }
            else if (levelTrack == 2)
            {
                LevelNumberCount = 3;
                InitLevel(level3Data);
            }
            else if (levelTrack == 3)
            {
                LevelNumberCount = 1;
                InitLevel(level4Data);
            }
            else if (levelTrack == 4)
            {
                LevelNumberCount = 2;
                InitLevel(level5Data);
            }
            else if (levelTrack == 5)
            {
                LevelNumberCount = 3;
                InitLevel(level6Data);
            }
            else if (levelTrack == 6)
            {
                LevelNumberCount = 4;
                InitLevel(level7Data);
            }
            else if (levelTrack == 7)
            {
                LevelNumberCount = 1;
                InitLevel(level8Data);
            }
            else if (levelTrack == 8)
            {
                LevelNumberCount = 2;
                InitLevel(level9Data);
            }
            else if (levelTrack == 9)
            {
                LevelNumberCount = 1;
                InitLevel(level10Data);
            }
        }

        public void InitLevel(LevelData levelData)
        {
            // Initialize the level using the provided LevelData
            titleText.text = GameManagement.Instance.stageName + " - " + LevelNumberCount;
            currentLevelData = levelData;
            ClearNodes();
            SpawnBoard();
            SpawnNodes();
        }

        //Increases the size of the board by 1x1
        public void incrementStage() 
        {
            GameManagement.Instance.currentStage++;
        }

        public void ClearNodes()
        {
            if (_nodes == null) return;

            foreach (var node in _nodes)
            {
                Destroy(node.gameObject);
            }

            _nodes.Clear();
            _nodeGrid.Clear();
        }

        //counter for number of times clickedLoadNextLevel is called
        public int functionCallCounter = 0;

        // Fetch Level Data from GameManagement
        LevelData level2Data = GameManagement.Instance.GetLevelData("Level2");
        LevelData level3Data = GameManagement.Instance.GetLevelData("Level3");
        LevelData level4Data = GameManagement.Instance.GetLevelData("Level4");
        LevelData level5Data = GameManagement.Instance.GetLevelData("Level5");
        LevelData level6Data = GameManagement.Instance.GetLevelData("Level6");
        LevelData level7Data = GameManagement.Instance.GetLevelData("Level7");
        LevelData level8Data = GameManagement.Instance.GetLevelData("Level8");
        LevelData level9Data = GameManagement.Instance.GetLevelData("Level9");
        LevelData level10Data = GameManagement.Instance.GetLevelData("Level10");
        public void clickedLoadNextLevel()
        {
            if (!hasGameFinished) return;
            hasGameFinished = false;
            winText.SetActive(false);

            if (functionCallCounter == 0)
            {
                LevelNumberCount = 2;
                if (level2Data == null)
                {
                    Debug.LogError("Level 2 data not found.");
                    return;
                }
                //Now a 6x6
                incrementStage();
                // Initialize the level2 using the retrieved LevelData
                InitLevel(level2Data);
            }
            else if (functionCallCounter == 1)
            {
                LevelNumberCount = 3;
                if (level3Data == null)
                {
                    Debug.LogError("Level 3 data not found.");
                    return;
                }
                // Initialize the level3 using the retrieved LevelData
                InitLevel(level3Data);
            }
            else if (functionCallCounter == 2)
            {
                GameManagement.Instance.stageName = "Easy";
                LevelNumberCount = 1;
                if (level4Data == null)
                {
                    Debug.LogError("Level 4 data not found.");
                    return;
                }
                //Now a 7x7
                incrementStage();
                // Initialize the level4 using the retrieved LevelData
                InitLevel(level4Data);
            }
            else if (functionCallCounter == 3)
            {
                LevelNumberCount = 2;
                if (level5Data == null)
                {
                    Debug.LogError("Level 5 data not found");
                }
                InitLevel(level5Data);
            }
            else if (functionCallCounter == 4)
            {
                LevelNumberCount = 3;
                if (level6Data == null)
                {
                    Debug.LogError("Level 6 data not found.");
                    return;
                }
                // Initialize the leve6 using the retrieved LevelData
                InitLevel(level6Data);
            }
            else if (functionCallCounter == 5)
            {
                LevelNumberCount = 4;
                if (level7Data == null)
                {
                    Debug.LogError("Level 7 data not found.");
                    return;
                }
                // Initialize the level7 using the retrieved LevelData
                InitLevel(level7Data);
            }
            else if (functionCallCounter == 6)
            {
                GameManagement.Instance.stageName = "Medium";
                LevelNumberCount = 1;
                if (level8Data == null)
                {
                    Debug.LogError("Level 8 data not found.");
                    return;
                }
                //Now a 8x8
                incrementStage();
                // Initialize the level8 using the retrieved LevelData
                InitLevel(level8Data);
            }
            else if (functionCallCounter == 7)
            {
                LevelNumberCount = 2;
                if (level9Data == null)
                {
                    Debug.LogError("Level 9 data not found.");
                    return;
                }
                // Initialize the level9 using the retrieved LevelData
                InitLevel(level9Data);
            }
            else if (functionCallCounter == 8)
            {
                GameManagement.Instance.stageName = "Hard";
                LevelNumberCount = 1;
                if (level10Data == null)
                {
                    Debug.LogError("Level 10 data not found.");
                    return;
                }
                //Now a 9x9
                incrementStage();
                // Initialize the level10 using the retrieved LevelData
                InitLevel(level10Data);
            }

            functionCallCounter++;
            LevelNumberCount++;
            levelTrack++;
        }
        //end of class
    }
    //end of Namespace
}