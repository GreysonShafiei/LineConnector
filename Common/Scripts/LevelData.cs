using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName =  "level", menuName = "SO/Level")]
    public class LevelData : ScriptableObject
    {
        public string LevelName;
        public List<Edge> Edges;
    }

    [System.Serializable]
    public struct Edge
    {
        public List<Vector2Int> Vertices;
        public Vector2Int Start
        {
            get
            {
                if (Vertices != null && Vertices.Count > 0)
                {
                    return Vertices[0];
                }
                return new Vector2Int(-1, -1);
            }
        }

        public Vector2Int End
        {
            get
            {
                if (Vertices != null && Vertices.Count > 0)
                {
                    return Vertices[Vertices.Count - 1];
                }
                return new Vector2Int(-1, -1);
            }
        }
    }
}