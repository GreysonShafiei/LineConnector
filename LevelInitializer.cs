using Connect.Common;
using Connect.Core;
using UnityEngine;


namespace Connect.Core
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] private LevelData levelData;

        void Start()
        {
            if (GameplayManagement.Instance != null)
            {
                GameplayManagement.Instance.InitLevel(levelData);
            }
        }
    }
}