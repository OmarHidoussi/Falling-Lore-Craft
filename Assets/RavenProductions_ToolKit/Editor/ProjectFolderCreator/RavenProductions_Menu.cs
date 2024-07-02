using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RavenProductions.Core
{
    public class RavenProductions_Menu : MonoBehaviour
    {

        #region Create Tools Menu
        [MenuItem("Raven Productions/Tools/Create Project Folders")]
        public static void CreateProjectFolders()
        {
            RavenProductions_ProjectFolders_Window.InitWindow();
        }

        [MenuItem("Raven Productions/Tools/Object Placer")]
        public static void ObjectPlacer()
        {
            RavenProductions_ObjectPlacer.InitWindow();
        }
        #endregion
    }
}