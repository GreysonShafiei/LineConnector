using UnityEngine;
using System.Collections.Generic;
using Connect.Core;

namespace Connect.Core
{
    public class LevelGenerator : MonoBehaviour
    {
        public GameObject nodePrefab;
        public Transform nodesParent;

        public Stage GenerateLevel(string levelName, Color levelColor, int stageNumber, int nodeCount)
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < nodeCount; i++)
            {
                GameObject nodeObj = Instantiate(nodePrefab, GetRandomPosition(), Quaternion.identity, nodesParent);
                Node node = nodeObj.GetComponent<Node>();
                nodes.Add(node);
            }

            GameObject stageObj = new GameObject(levelName);
            Stage stage = stageObj.AddComponent<Stage>();
            stage.stageName = levelName;
            stage.stageColor = levelColor;
            stage.stageNumber = stageNumber;

            return stage;
        }

        private Vector3 GetRandomPosition()
        {
            float xMin = -10f;
            float xMax = 10f;
            float yMin = -10f;
            float yMax = 10f;

            float x = Random.Range(xMin, xMax);
            float y = Random.Range(yMin, yMax);
            float z = 0f; // Assuming a 2D game

            return new Vector3(x, y, z);
            //return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }
    }
}
