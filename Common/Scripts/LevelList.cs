using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connect.Common;


namespace Connect.Core
{
    [CreateAssetMenu(fileName = "level", menuName = "SO/AllLevels")]
    public class LevelList : ScriptableObject
    {
        public List<LevelData> levels;
    }
}
