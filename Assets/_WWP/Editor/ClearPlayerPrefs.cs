using System.IO;
using UnityEditor;
using UnityEngine;

namespace GooyesPlugin.Editor
{
    public class ClearPlayerPrefs : MonoBehaviour
    {
        [MenuItem("GooyesPlugin/Clear/Player Prefs")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}