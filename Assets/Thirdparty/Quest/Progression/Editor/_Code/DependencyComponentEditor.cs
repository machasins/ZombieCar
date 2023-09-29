using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Progression;
using System.Reflection;
using System.Linq;

namespace ProgressionEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuestDependencyComponent))]
    public class DependencyComponentEditor : Editor
    {
        private static string[] typeNames;
        private Vector2 scrollPos;
        private SerializedProperty questDependencySerializedProperty;
        private static FieldInfo fieldInfo;

        public void OnEnable()
        {
            if(typeNames == null)
            {
                System.Type type = typeof(Dependency);
                List<System.Type> types = new List<System.Type>();
                Assembly current = System.Reflection.Assembly.GetAssembly(type);
                List<Assembly> sub;
                sub = UnityEditor.Compilation.CompilationPipeline.GetAssemblies().Where(x => x != null && x.assemblyReferences.Any(y => y != null && y.name == current.GetName().Name)).Select(z => z != null ? Assembly.Load(z.name) : null).ToList();
                sub = sub.Where(y => y.GetReferencedAssemblies().Any((AssemblyName z) => { return z.FullName == current.GetName().FullName; })).ToList();
                sub.Add(current);
                types.AddRange(sub.SelectMany(x => x.GetTypes().Where(m =>type.IsAssignableFrom(m))));
                types.Remove(type);
                typeNames = types.Select(x => x.Name).ToArray();
                fieldInfo = typeof(QuestDependencyComponent).GetField("questDependency", BindingFlags.NonPublic |BindingFlags.Public| BindingFlags.Instance);

            }
            questDependencySerializedProperty = serializedObject.FindProperty("questDependency");
        }

        public void CreateNewDependencyOfType(string typeName)
        {

            bool auto = EditorUtility.DisplayDialog("Auto Save", "Would you like to automatically name and save a dependencies?", "Automatic", "Manual");
            UnityEngine.Object[] targetObjects = serializedObject.targetObjects;
            int length = serializedObject.targetObjects.Length;
            for (int i = 0; i < length; i++)
            {
                QuestDependencyComponent dependency = (QuestDependencyComponent)targetObjects[i];
                if (!dependency || questDependencySerializedProperty.objectReferenceValue || !dependency.gameObject.scene.IsValid())
                    continue;
                string fileName = dependency.gameObject.name + "_" + typeName;
                string finalPath = "";
                string scenePath = AssetDatabase.GetAssetOrScenePath(targetObjects[i]);
                string destFolder = scenePath.Remove(scenePath.Length - 6, 6) + "_DependencyAssets";
                if (auto)
                {
                    if (!AssetDatabase.IsValidFolder(destFolder))
                    {
                        destFolder = AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(scenePath), System.IO.Path.GetFileNameWithoutExtension(scenePath) + "_DependencyAssets");
                    }
                    finalPath = destFolder + "/" + fileName + "_" + ".asset";
                    finalPath = AssetDatabase.GenerateUniqueAssetPath(finalPath);
                }
                else
                {
                    bool hasCancelled = false;
                    while (!hasCancelled && finalPath == "")
                    {
                        scenePath = AssetDatabase.GetAssetOrScenePath(targetObjects[i]);
                        destFolder = scenePath.Remove(scenePath.Length - 6, 6) + "_DependencyAssets";
                        if (!AssetDatabase.IsValidFolder(destFolder))
                        {
                            destFolder = AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(scenePath), System.IO.Path.GetFileNameWithoutExtension(scenePath) + "_DependencyAssets");
                        }
                        finalPath = destFolder + "/" + fileName + "_" + ".asset";
                        finalPath = EditorUtility.SaveFilePanelInProject("Dependency Asset", fileName, "asset", "Save This Dependency Asset", destFolder);
                        if (finalPath == null || finalPath == "")
                        {
                            hasCancelled = EditorUtility.DisplayDialog("Warning", "Are you sure you don't want to create a Dependency Asset for this component?", "Yes", "No");
                        }
                    }
                    if (hasCancelled)
                        if (i < length - 1 && EditorUtility.DisplayDialog("Cancel All", "Would you like to quit making dependecy assets for the rest of the objects?", "Yes", "No"))
                            return;
                        else
                            continue;
                }

                Object p = ScriptableObject.CreateInstance(typeName);
                p.name = System.IO.Path.GetFileNameWithoutExtension(finalPath);
                AssetDatabase.CreateAsset(p, finalPath);
                AssetDatabase.ImportAsset(finalPath);
                Undo.RecordObject(dependency, "Assign new Dependency");
                fieldInfo.SetValue(dependency, p);

            }
            serializedObject.Update();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
                return;
            GUILayout.Label("Create New Dependency");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(Mathf.Min(typeNames.Length*22.0f, 150.0f)));
            for(int i = 0; i < typeNames.Length; i++)
            {
                if (GUILayout.Button(typeNames[i]))
                {
                    CreateNewDependencyOfType(typeNames[i]);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}