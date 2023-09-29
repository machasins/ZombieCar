using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Progression;


namespace ProgressionEditor
{

    [CustomEditor(typeof(QuestManagerAsset))]
    public class QuestManagerEditor : BaseScriptableObjectEditor
    {
        //ProgressionPoints
        private SerializedProperty progressionPointsProperty;
        private ReorderableList progressionPointsReorderableList;
        private ProgressionPointEditor progressionEditor;

        //Quests
        private SerializedProperty questsProperty;
        private ReorderableList questsReorderableList;
        private QuestAssetEditor questEditor;
        private HashSet<string> questNames = new HashSet<string>();

        private Vector2 scrollPos;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!UseCustom) 
                return;
            OnQuestsGUI();
        }

        public void OnDisable()
        {
            if (questEditor != null)
                DestroyImmediate(questEditor);

            Object main = AssetDatabase.LoadMainAssetAtPath(AssetPath);
            QuestManagerAsset questManager = (QuestManagerAsset)main;
            if (questManager == null)
                return;
            QuestAsset[] quests = questManager.GetQuests();
            for (int i = 0; i < quests.Length; i++)
                if (AssetPath != AssetDatabase.GetAssetPath(quests[i]))
                    AssetDatabase.AddObjectToAsset(quests[i], questManager);
            ProgressionPointAsset[] progressionPoints = questManager.GetProgressionPoints();
            for (int i = 0; i < progressionPoints.Length; i++)
                if (AssetPath != AssetDatabase.GetAssetPath(progressionPoints[i]))
                    AssetDatabase.AddObjectToAsset(progressionPoints[i], questManager);
            if (AssetPath != null && AssetPath != "")
                EditorApplication.delayCall+= ()=>{AssetDatabase.ImportAsset(AssetPath); };
        }

        #region questlines

        public void OnQuestsEnable()
        {
            questsProperty = questsProperty == null ? serializedObject.FindProperty("quests") : questsProperty;
            if (questsReorderableList == null)
            {
                questsReorderableList = new ReorderableList(serializedObject, questsProperty, true, true, true, true);
                questsReorderableList.drawElementCallback += OnQuestElementGUI;
                questsReorderableList.elementHeightCallback += GetQuestElementHeight;
                questsReorderableList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Quests"); };
                questsReorderableList.onAddCallback += OnQuestAddElement;
                questsReorderableList.onRemoveCallback += OnQuestRemoveElement;
                questsReorderableList.onSelectCallback += OnQuestSelectElement;
            }
        }

        public void OnQuestsGUI()
        {
            if (questsProperty == null || questsReorderableList == null)
                OnQuestsEnable();
            float inspectorHeight = Screen.height *EditorGUIUtility.currentViewWidth/Screen.width;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight((inspectorHeight - EditorGUIUtility.singleLineHeight*18.0f)/3.0f));
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            questsReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                questNames.Clear();
                questNames.UnionWith(((QuestManagerAsset)serializedObject.targetObject).GetQuests().Select(x => (x==null?null: x.name)));
            }
            EditorGUILayout.EndScrollView();
            if (questEditor != null && questEditor.target != null)
            {
                questEditor.DrawHeader();
                questEditor.OnInspectorGUI();
            }
        }

        public void OnQuestAddElement(ReorderableList reorderableList)
        {
            Undo.RecordObject(serializedObject.targetObject, "Add Quest Item");
            ReorderableList.defaultBehaviours.DoAddButton(reorderableList);
            serializedObject.ApplyModifiedProperties();
            QuestAsset p = CreateAsset<QuestAsset>(assetName: GetUniqueQuestName(typeof(QuestAsset).Name));
            reorderableList.serializedProperty.GetArrayElementAtIndex(reorderableList.serializedProperty.arraySize - 1).objectReferenceValue = p;
            serializedObject.ApplyModifiedProperties();
        }

        public void OnQuestRemoveElement(ReorderableList reorderableList)
        {
            Undo.RecordObject(serializedObject.targetObject, "Delete Quest Item");
            if (questEditor != null)
                DestroyImmediate(questEditor);
            QuestManagerAsset st = ((QuestManagerAsset)serializedObject.targetObject);
            QuestAsset p = st.GetQuests()[reorderableList.index];
            ProgressionPointAsset[] progressionPoints = p.GetProgressionPoints();
            for (int i=0; i< progressionPoints.Length; i++)
                DestroyAsset(progressionPoints[i]);
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(reorderableList.index);
            reorderableList.serializedProperty.MoveArrayElement(reorderableList.index, reorderableList.count - 1);
            reorderableList.serializedProperty.arraySize--;
            if (p != null)
                DestroyAsset(p);
        }

        public void OnQuestSelectElement(ReorderableList reorderableList)
        {
            SerializedProperty serializedProperty = questsProperty.GetArrayElementAtIndex(reorderableList.index);
            if (questEditor != null)
                DestroyImmediate(questEditor);
            questEditor = (QuestAssetEditor)Editor.CreateEditor(serializedProperty.objectReferenceValue, typeof(QuestAssetEditor));
        }



        public void OnQuestElementGUI(Rect rect, int index, bool isActive, bool isFocused)
        {

            SerializedProperty serializedProperty = questsProperty.GetArrayElementAtIndex(index);
            QuestAsset quest = serializedProperty.objectReferenceValue as QuestAsset;
            serializedProperty.isExpanded = true;
            if (quest == null)
                return;
            if (isActive)
            {
                int original = GUI.skin.textField.fontSize;
                GUI.skin.textField.fontSize = original * 2;
                EditorGUI.BeginChangeCheck();
                GUIStyle style = new GUIStyle(GUI.skin.textField);
                style.fontSize = original * 2;
                GUI.SetNextControlName(quest.GetInstanceID().ToString());
                string newName = EditorGUI.DelayedTextField(rect, "", quest.name, style);
                if (GUI.GetNameOfFocusedControl() == quest.GetInstanceID().ToString())
                {
                    Rect helpRect = new Rect(rect.x, rect.y - EditorGUIUtility.singleLineHeight, rect.width, rect.height/2);
                    EditorGUI.HelpBox(helpRect, "Press Enter To Submit", MessageType.Warning);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    newName = GetUniqueQuestName(newName);
                    Undo.RecordObject(quest, "Change name from " + quest.name + " to " + newName);

                    quest.name = newName;
                    EditorUtility.SetDirty(quest);
                    serializedObject.ApplyModifiedProperties();
                    questsReorderableList.onSelectCallback(questsReorderableList);
                }
                GUI.skin.textField.fontSize = original;
            }
            else
                EditorGUI.LabelField(rect, quest.name);

        }

        private string GetUniqueQuestName(string newName)
        {
            string copyOfNewName = newName;
            newName = new string(copyOfNewName.Where(x => !System.IO.Path.GetInvalidFileNameChars().Contains(x)).ToArray());
            if (questNames.Contains(newName))
            {
                int i = 1;
                for (; questNames.Contains(newName + "_" + i.ToString()); i++)
                {

                }
                newName = newName + "_" + i.ToString();
            }
            return newName;
        }

        public float GetQuestElementHeight(int index)
        {
            float height = EditorGUIUtility.singleLineHeight * 2;
            return height;
        }
        #endregion
    }


    [CustomEditor(typeof(QuestAsset))]
    public class QuestAssetEditor : BaseScriptableObjectEditor
    {
        private const string ProgressionPointHelpBoxText = "Progression points are achieved in order, and after their dependencies are satisfied.";

        //ProgressionPoints
        private SerializedProperty progressionPointsProperty;
        private ReorderableList progressionPointsReorderableList;
        private ProgressionPointEditor progressionEditor;

        private HashSet<string> progressionPointNames = new HashSet<string>();

        private Vector2 scrollPos;

        public void OnEnable()
        {
            StripNames();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!UseCustom)
                return;
            OnProgressionPointsGUI();
        }

        public void OnDisable()
        {
            QuestAsset quest = (QuestAsset)serializedObject.targetObject;
            if (quest == null)
                return;
            PrefixNames();
            ProgressionPointAsset[] progressionPointArray = quest.GetProgressionPoints();
            for (int i = 0; i < progressionPointArray.Length; i++)
                if (AssetPath != AssetDatabase.GetAssetPath(progressionPointArray[i]))
                    AssetDatabase.AddObjectToAsset(progressionPointArray[i], quest);
            EditorApplication.delayCall+=()=> AssetDatabase.ImportAsset(AssetPath);
        }

        public void OnProgressionPointsEnable()
        {
            progressionPointsProperty = progressionPointsProperty == null ? serializedObject.FindProperty("progressionPoints") : progressionPointsProperty;
            if (progressionPointsReorderableList == null)
            {
                progressionPointsReorderableList = new ReorderableList(serializedObject, progressionPointsProperty, true, true, true, true);
                progressionPointsReorderableList.drawElementCallback += OnProgressionPointElementGUI;
                progressionPointsReorderableList.elementHeightCallback += GetProgressionPointElementHeight;
                progressionPointsReorderableList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "ProgressionPoints"); };
                progressionPointsReorderableList.onAddCallback += OnProgressionPointAddElement;
                progressionPointsReorderableList.onRemoveCallback += OnProgressionPointRemoveElement;
                progressionPointsReorderableList.onSelectCallback += OnProgressionPointSelectElement;
                progressionPointNames = new HashSet<string>(((QuestAsset)target)? ((QuestAsset)target).GetProgressionPoints().Select(x => x.name):new string[0]);
            }
        }

        public void OnProgressionPointsGUI()
        {
            if (progressionPointsProperty == null || progressionPointsReorderableList == null)
                OnProgressionPointsEnable();
            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(0));
            windowRect.height = 6000;
            windowRect.x -= 2;
            windowRect.width += 4;
            EditorGUI.DrawRect(windowRect, Color.black / 20);
            float inspectorHeight = Screen.height * EditorGUIUtility.currentViewWidth / Screen.width;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight((inspectorHeight - EditorGUIUtility.singleLineHeight * 18.0f) / 3.0f));
            EditorGUILayout.HelpBox(ProgressionPointHelpBoxText, MessageType.Info, true);
            DropAreaGUI();
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            progressionPointsReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            if (progressionEditor != null && progressionEditor.target != null)
            {
                progressionEditor.DrawHeader();
                progressionEditor.OnInspectorGUI();
            }
        }

        public void OnProgressionPointSelectElement(ReorderableList reorderableList)
        {
            SerializedProperty serializedProperty = progressionPointsProperty.GetArrayElementAtIndex(reorderableList.index);
            progressionEditor = (ProgressionPointEditor)Editor.CreateEditor(serializedProperty.objectReferenceValue, typeof(ProgressionPointEditor));
        }


        public void OnProgressionPointAddElement(ReorderableList reorderableList)
        {
            Undo.RegisterCompleteObjectUndo(serializedObject.targetObject, "Add ProgressionItem");
            ReorderableList.defaultBehaviours.DoAddButton(reorderableList);
            serializedObject.ApplyModifiedProperties();
            ProgressionPointAsset p = CreateAsset<ProgressionPointAsset>();
            p.name = GetUniqueProgressionPointName("ProgressionPointAsset");
            progressionPointNames.Add(p.name);
            reorderableList.serializedProperty.GetArrayElementAtIndex(reorderableList.serializedProperty.arraySize - 1).objectReferenceValue = p;
            serializedObject.ApplyModifiedProperties();
        }

        public void OnProgressionPointRemoveElement(ReorderableList reorderableList)
        {
            Undo.RegisterCompleteObjectUndo(serializedObject.targetObject, "Delete ProgressionItem");
            ProgressionPointAsset p = ((QuestAsset)serializedObject.targetObject).GetProgressionPoints()[reorderableList.index];
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(reorderableList.index);
            reorderableList.serializedProperty.MoveArrayElement(reorderableList.index, reorderableList.count - 1);
            reorderableList.serializedProperty.arraySize--;
            progressionPointNames.Remove(p.name);
            if (p != null)
                DestroyAsset(p);
            serializedObject.ApplyModifiedProperties();
            if(progressionEditor != null)
            {
                DestroyImmediate(progressionEditor);
            }
        }

        private void StripNames()
        {
            QuestAsset quest = ((QuestAsset)serializedObject.targetObject);
            if (quest == null)
                return;
            ProgressionPointAsset[] progressionPointArray = quest.GetProgressionPoints();
            for (int i = 0; i < progressionPointArray.Length; i++)
            {
                ProgressionPointAsset p = progressionPointArray[i];
                string intFormat = new string('0', (progressionPointArray.Length / 10)+1);
                p.name = p.name.Replace(quest.name + "_" + i.ToString(intFormat)+"|", "");
            }
            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        private void PrefixNames()
        {
            QuestAsset quest = ((QuestAsset)serializedObject.targetObject);
            if (quest == null)
                return;
            ProgressionPointAsset[] progressionPointArray = quest.GetProgressionPoints();
            for (int i = 0; i < progressionPointArray.Length; i++)
            {
                ProgressionPointAsset p = progressionPointArray[i];
                string intFormat = new string('0', (progressionPointArray.Length / 10)+1);
                p.name = quest.name + "_" + i.ToString(intFormat) + "|" + p.name;
            }
            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        public void OnProgressionPointElementGUI(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect topRect = rect;
            topRect.y -= EditorGUIUtility.singleLineHeight / 2;
            EditorGUI.LabelField(topRect, index.ToString());
            rect.x += 20;
            SerializedProperty serializedProperty = progressionPointsProperty.GetArrayElementAtIndex(index);
            ProgressionPointAsset progressionPoint = (ProgressionPointAsset)serializedProperty.objectReferenceValue;
            serializedProperty.isExpanded = true;

            if (isActive)
            {
                int original = GUI.skin.textField.fontSize;
                GUI.skin.textField.fontSize = original * 2;
                EditorGUI.BeginChangeCheck();
                GUIStyle style = new GUIStyle(GUI.skin.textField);
                style.fontSize = original * 2;
                GUI.SetNextControlName(progressionPoint.GetInstanceID().ToString());
                string newName = EditorGUI.DelayedTextField(rect, "", progressionPoint.name, style);
                if (GUI.GetNameOfFocusedControl() == progressionPoint.GetInstanceID().ToString())
                {
                    Rect helpRect = new Rect(rect.x, rect.y - EditorGUIUtility.singleLineHeight, rect.width, rect.height / 2);
                    EditorGUI.HelpBox(helpRect, "Press Enter To Submit", MessageType.Warning);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    progressionPointNames.Remove(progressionPoint.name);
                    newName = GetUniqueProgressionPointName(newName);
                    progressionPointNames.Add(newName);
                    Undo.RecordObject(progressionPoint, "Change name from " + progressionPoint.name + " to " + newName);
                    progressionPoint.name = newName;
                    EditorUtility.SetDirty(progressionPoint);
                    serializedObject.ApplyModifiedProperties();
                    progressionPointsReorderableList.onSelectCallback(progressionPointsReorderableList);
                }
                GUI.skin.textField.fontSize = original;
            }
            else
                EditorGUI.LabelField(rect, progressionPoint?progressionPoint.name:"");
        }

        private string GetUniqueProgressionPointName(string newName)
        {
            string copyOfNewName = newName;
            newName = new string(copyOfNewName.Where(x => !System.IO.Path.GetInvalidFileNameChars().Contains(x)).ToArray());
            if (progressionPointNames.Contains(newName))
            {
                int i = 1;
                for (; progressionPointNames.Contains(newName + "_" + i.ToString()); i++)
                {

                }
                newName = newName + "_" + i.ToString();
            }
            return newName;
        }

        public float GetProgressionPointElementHeight(int index)
        {

            float height = EditorGUIUtility.singleLineHeight * 2;
            return height;
        }

        private void DropAreaGUI()
        {

            if (progressionPointsProperty == null)
                return;
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
            drop_area.height += (progressionPointsProperty.arraySize + 1) * (EditorGUIUtility.singleLineHeight*2 + 2);
            GUI.Box(drop_area, "Drag and Drop Progression Points");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            // Do On Drag Stuff here
                            ProgressionPointAsset dependency = (ProgressionPointAsset)dragged_object;
                            if (dependency != null)
                            {
                                Undo.RegisterCompleteObjectUndo(target, "Add dependency to progression point");
                                progressionPointsProperty.InsertArrayElementAtIndex(Mathf.Max(0, progressionPointsProperty.arraySize - 1));
                                progressionPointsProperty.GetArrayElementAtIndex(progressionPointsProperty.arraySize - 1).objectReferenceValue = dragged_object;
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                        return;
                    }
                    break;
            }
        }
    }

    [CustomEditor(typeof(ProgressionPointAsset))]
    public class ProgressionPointEditor : BaseScriptableObjectEditor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty dependenciesProperty;
        private QuestManagerAsset manager;
        private int[] questIndices = new int[0];


        private void OnEnable()
        {
            manager = AssetDatabase.LoadAssetAtPath<QuestManagerAsset>(AssetPath);
            QuestAsset[] quests = manager.GetQuests();
            questIndices = new int[quests.Length];
            for (int i = 0; i < questIndices.Length; i++)
                questIndices[i] = i;
            dependenciesProperty = serializedObject.FindProperty("dependencies");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!UseCustom)
                return;
            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(0));
            windowRect.height = 6000;
            windowRect.x -= 1;
            windowRect.width += 2;
            EditorGUI.DrawRect(windowRect, Color.black / 20);
            DropAreaGUI();
        }

        private void DropAreaGUI()
        {

            if (dependenciesProperty == null)
                return;
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
            drop_area.height += (dependenciesProperty.arraySize+3 )* (EditorGUIUtility.singleLineHeight+2);
            GUI.Box(drop_area, "Drag and Drop Dependencies");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            // Do On Drag Stuff here
                            Dependency dependency = (Dependency) dragged_object;
                            if (dependency != null)
                            {
                                Undo.RegisterCompleteObjectUndo(target, "Add dependency to progression point");
                                dependenciesProperty.InsertArrayElementAtIndex(Mathf.Max(0, dependenciesProperty.arraySize - 1));
                                dependenciesProperty.GetArrayElementAtIndex(dependenciesProperty.arraySize - 1).objectReferenceValue = dragged_object;
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                        return;
                    }
                    break;
            }
            for(int i =dependenciesProperty.arraySize-1; i >=0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(dependenciesProperty.GetArrayElementAtIndex(i).objectReferenceValue?dependenciesProperty.GetArrayElementAtIndex(i).objectReferenceValue.name:"");
                if (GUILayout.Button("X"))
                {
                    Undo.RegisterCompleteObjectUndo(target, "Remove Dependency");
                    dependenciesProperty.MoveArrayElement(i, dependenciesProperty.arraySize - 1);
                    dependenciesProperty.arraySize--;
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    [CustomEditor(typeof(BaseScriptableObject), true)]
    public class BaseScriptableObjectEditor : Editor
    {
        public bool UseCustom = true;

        //ScriptableSaving
        private string assetPath;

        public string AssetPath
        {
            get
            {
                if (assetPath == null || assetPath == "")
                    if (serializedObject == null)
                        assetPath = "";
                    else
                        assetPath = AssetDatabase.GetAssetPath(serializedObject.targetObject);
                return assetPath;
            }
        }


        public override void OnInspectorGUI()
        {
            if (GetType() == typeof(BaseScriptableObjectEditor) || !UseCustom)
                base.OnInspectorGUI();
        }

        protected T CreateAsset<T>(string message = "Added new asset ", string assetName = "name") where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = assetName;
            Undo.RegisterCreatedObjectUndo(asset, message + name);
            AssetDatabase.AddObjectToAsset(asset, AssetPath);
            AssetDatabase.ImportAsset(AssetPath);
            return asset;
        }

        protected void DestroyAsset(ScriptableObject asset, string message = "Removed asset from questlineManager")
        {
            Undo.RegisterCompleteObjectUndo(AssetDatabase.LoadAssetAtPath(AssetPath, typeof(QuestManagerAsset)), message);
            if (Application.isPlaying)
                Destroy(asset);
            else
                Undo.DestroyObjectImmediate(asset);
            DestroyImmediate(asset, true);
            AssetDatabase.ImportAsset(AssetPath);
        }
    }


}