using UnityEditor;
using UnityEngine;


namespace RavenProductions.Core
{
    public class RavenProductions_BaseWindow : EditorWindow
    {
        #region Variables
        protected GameObject[] selectedGameObject = new GameObject[0];
        #endregion

        #region Methode
        protected virtual void GetSelected()
        {
            selectedGameObject = Selection.gameObjects;
        }
        #endregion
    }
}