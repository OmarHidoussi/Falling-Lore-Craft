using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

namespace RavenProductions.Core
{
    public class RavenProductions_ObjectPlacer : RavenProductions_BaseWindow
    {
        #region Variables
        static RavenProductions_ObjectPlacer m_win;

        #endregion
        public static void InitWindow()
        {
            m_win = GetWindow<RavenProductions_ObjectPlacer>(true, "Object Placer", true);
            m_win.minSize = new Vector2(290, 480);
            m_win.maxSize = new Vector2(290, 480);

            m_win.minSize = new Vector2(290, 300);
            m_win.maxSize = new Vector2(4000, 4000);

            m_win.titleContent = new GUIContent("Object Placer");
            m_win.titleContent.image = Resources.Load<Texture2D>("OP_Icon");

            m_win.Show();
        }

        private void OnEnable()
        {
            // Reference to the root of the window
            var root = this.rootVisualElement;

            // Assign a stylesheet to the root and its children
            root.styleSheets.Add(Resources.Load<StyleSheet>("ObjectPlacer"));

            // Loads and clones our VisualTree (Our UXML structure) inside the root
            var ObjectPlacerVisualTree = Resources.Load<VisualTreeAsset>("ObjectPlacer"); 
            ObjectPlacerVisualTree.CloneTree(root);

            // Change selection zone to material
            root.Q<ObjectField>("GameObject-Picker").objectType = typeof(GameObject);

            // Change selection zone to scene object
            root.Q<ObjectField>("Parent-Picker").objectType = typeof(GameObject);

            SceneView.duringSceneGui += SceneViewUpdate;
        }

        private void OnDestroy()
        {
            // Remove the function to stop the tool
            SceneView.duringSceneGui -= SceneViewUpdate;
        }

        // Detect click events in scene
        void SceneViewUpdate(SceneView sceneView)
        {
            //Make a reference to the current event
            var m_event = Event.current;
            Toggle enableToggle = (Toggle)this.rootVisualElement.Q<Toggle>("Enabled-Toggle");
            ObjectField objectSelector = (ObjectField)this.rootVisualElement.Q<ObjectField>("GameObject-Picker");

            if(m_event.type == EventType.MouseDown && m_event.button == 0
                && enableToggle.value == true && objectSelector.value != null
                && !m_event.alt)
            {
                InstantiateGameObject((GameObject)objectSelector.value);
            }

            if(m_event.type == EventType.Used && enableToggle.value == true
                && objectSelector.value != null && !m_event.alt)
            {
                Selection.activeGameObject = null;
            }
        }

        void InstantiateGameObject(GameObject gameObject)
        {

            //Get User Options
            string name = (string)this.rootVisualElement.Q<TextField>("Name-Field").value;
            bool nameToggle = (bool)this.rootVisualElement.Q<Toggle>("Name-Toggle").value;
            bool staticToggle = (bool)this.rootVisualElement.Q<Toggle>("Static-Toggle").value;
            GameObject parent = (GameObject)this.rootVisualElement.Q<ObjectField>("Parent-Picker").value;
            String tag = (String)this.rootVisualElement.Q<TagField>("Tag-Field").value;
            int layer = (int)this.rootVisualElement.Q<LayerField>("Layer-Field").value;

            // Position Options
            Vector3 offset = (Vector3)this.rootVisualElement.Q<Vector3Field>("Offset-Field").value;
            bool alignToggle = (bool)this.rootVisualElement.Q<Toggle>("Align-Toggle").value;

            // Rotation Options
            Vector3 rotationMin = (Vector3)this.rootVisualElement.Q<Vector3Field>("RotationMin-Field").value;
            Vector3 rotationMax = (Vector3)this.rootVisualElement.Q<Vector3Field>("RotationMax-Field").value;
            bool normalToggle = (bool)this.rootVisualElement.Q<Toggle>("Normal-Toggle").value;

            // Scale Options
            Vector3 scaleMin = (Vector3)this.rootVisualElement.Q<Vector3Field>("ScaleMin-Field").value;
            Vector3 scaleMax = (Vector3)this.rootVisualElement.Q<Vector3Field>("ScaleMax-Field").value;
            bool scaleUniformToggle = (bool)this.rootVisualElement.Q<Toggle>("ScaleUniform-Toggle").value;

            // Cast a ray from GUI point
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit hit;
            Vector3 mousePosition = Physics.Raycast(ray, out hit) ? hit.point : Vector3.negativeInfinity;
            if(mousePosition.x != float.NegativeInfinity)
            {
                //Allign to grid of 1 or not
                mousePosition = alignToggle ? new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), Mathf.Round(mousePosition.z)) : mousePosition;

                GameObject spawned;

                // Check if the reference is from a prefab, and spawn it as one if it should be
                if (PrefabUtility.IsPartOfAnyPrefab(gameObject) == true)
                {
                    gameObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                    spawned = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;
                    spawned.transform.position = mousePosition + offset;
                }
                else
                {
                    // Instantiate with offset and rotation
                    spawned = Instantiate(gameObject, mousePosition + offset, Quaternion.identity);
                }

                if (normalToggle)
                {
                    spawned.transform.rotation = Quaternion.FromToRotation(spawned.transform.forward, hit.normal);
                    spawned.transform.localRotation *= Quaternion.Euler(90, 0, 0);
                }

                // Rotate randomly
                Quaternion rotation = Quaternion.Euler(UnityEngine.Random.Range(rotationMin.x, rotationMax.x), UnityEngine.Random.Range(rotationMin.y, rotationMax.y), UnityEngine.Random.Range(rotationMin.z, rotationMax.z));
                spawned.transform.localRotation *= rotation;

                // Scale randomly
                if (scaleUniformToggle)
                {
                    Vector3 randomScale = Vector3.one * UnityEngine.Random.Range(scaleMin.x, scaleMax.x);
                    spawned.transform.localScale = randomScale;
                }
                else
                {
                    spawned.transform.localScale = new Vector3(UnityEngine.Random.Range(scaleMin.x, scaleMax.x), UnityEngine.Random.Range(scaleMin.y, scaleMax.y), UnityEngine.Random.Range(scaleMin.z, scaleMax.z));
                }

                // Set the name if the toggle is ticked
                spawned.name = nameToggle ? name : gameObject.name;

                // Set the other properties
                spawned.isStatic = staticToggle ? true : false;

                // Make sure the parent isnt accidentally an asset from folder
                if (parent != null)
                {
                    if (!AssetDatabase.Contains(parent))
                    {
                        spawned.transform.parent = parent.transform;
                    }
                    else { Debug.Log("Can't parent the new object to an asset file."); }
                }

                spawned.tag = tag;
                spawned.layer = layer;

                // Allow the created object to be undone
                Undo.RegisterCreatedObjectUndo(spawned, "Instantiate Object");
            }
        }
    }
}
