using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;


namespace RavenProductions.Core
{
    public class RavenProductions_ProjectFolders_Window : RavenProductions_BaseWindow
    {
        #region Variables
        static RavenProductions_ProjectFolders_Window m_win;

        string myRootName = "Game";
        string m_dialogue = "ProjectSetup";
        #endregion

        #region Main methods
        public static void InitWindow()
        {
            m_win = GetWindow<RavenProductions_ProjectFolders_Window>(true, "ProjectFolders", true);
            m_win.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Game Name: ", EditorStyles.boldLabel);
            myRootName = EditorGUILayout.TextField(myRootName);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Create Folders Structure", GUILayout.ExpandWidth(true), GUILayout.Height(32)))
            {
                CreatRootFolder();
            }
        }

        void CreatRootFolder()
        {
            if(myRootName == "" || myRootName == null)
            {
                DialogeDisplay("Please Provide a Proper Name");
                return;
            }
            if(myRootName == "Game")
            {
                DialogeDisplay("So XD you want to name your game .. Game?");
            }

            Debug.Log("Creating Root Folder ...");
            string assetsFolder = Application.dataPath;
            string rootname = assetsFolder + "/" + myRootName;

            DirectoryInfo rootInfo = Directory.CreateDirectory(rootname);

            if (!rootInfo.Exists)
            {
                return;
            }

            CreateSubDirectories(rootname);

            AssetDatabase.Refresh();

            if (m_win)
            {
                m_win.Close();
            }
        }

        void CreateSubDirectories(string rootFolder)
        {
            DirectoryInfo rootInfo = null;
            List<string> folderList = new List<string>();

            rootInfo = Directory.CreateDirectory(rootFolder + "/" + "Art");
            if (rootInfo.Exists)
            {
                folderList.Clear();
                folderList.Add("Animations");
                folderList.Add("Animators");
                folderList.Add("AudioMixers");
                folderList.Add("AudioClips");
                folderList.Add("Fonts");
                folderList.Add("Materials");
                folderList.Add("PhysicsMaterials");
                folderList.Add("Objects");
                folderList.Add("Textures");

                CreatSubFolders(rootFolder + "/" + "Art",folderList);
            }

            rootInfo = Directory.CreateDirectory(rootFolder + "/" + "Code");
            if (rootInfo.Exists)
            {
                folderList.Clear();
                folderList.Add("Editor");
                folderList.Add("Scripts");
                folderList.Add("Shaders");

                CreatSubFolders(rootFolder + "/" + "Code",folderList);     
            }

            rootInfo = Directory.CreateDirectory(rootFolder + "/" + "Resources");
            if (rootInfo.Exists)
            {
                folderList.Clear();
                folderList.Add("Characters");
                folderList.Add("Props");
                folderList.Add("UI");

                CreatSubFolders(rootFolder + "/" + "Resources", folderList);
            }

            DirectoryInfo ScenesDirectorie = Directory.CreateDirectory(rootFolder + "/" + "Scenes");

            Scene currentScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(currentScene, "Assets/" + myRootName + "/Scenes/" + myRootName + "_Startup.Unity", true);

            Scene MainScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(MainScene, "Assets/" + myRootName + "/Scenes/" + myRootName + "_Main.Unity", true);

        }

        void CreatSubFolders(string rootFolder,List<string> subFolders)
        {
            foreach(string folder in subFolders)
            {
                Directory.CreateDirectory(rootFolder + "/" + folder);
            }
        }

        void DialogeDisplay(string message)
        {
            EditorUtility.DisplayDialog(m_dialogue + "Warning", message,"OK");
        }
        #endregion
    }
}
