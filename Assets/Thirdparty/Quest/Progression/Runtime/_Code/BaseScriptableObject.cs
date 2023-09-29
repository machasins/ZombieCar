
using UnityEngine;
using System.IO;

/// <summary>
/// An extended class for scriptable objects to support saving and loading.
/// Slow to save, but really convenient.
/// Can not deserialize monobehavior/scriptableobject references.
/// </summary>
public class BaseScriptableObject : ScriptableObject, IOnPostProcessImportAssetCallback, IOnWillDeleteAssetCallback, IOnWillSaveAssetCallback
{
    [SerializeField]
    [HideInInspector]
    internal int saveSlot = -1;

    [Header("      [GUID]/")]
    [Header("    " + FolderName + "/")]
    [Header("  [Application.persistentDataPath]/")]
    [Header("File Directory Folder:")]
    [SerializeField]
    [HideInInspector]
    private string serializedGuid = "";

    private const string FileExtension = ".zip";

    private const string FolderName = "Progression";

    /// <summary>
    /// Save to a specified slot.
    /// </summary>
    /// <param name="slotToSaveTo">-1 is reserved for defaults</param>
    public void SaveToSlot(int slotToSaveTo)
    {
        if(slotToSaveTo == -1)
        {
            const string Message = "Save failed. Can not save to slot -1, -1 is reserved for defaults.";
            Debug.LogError(Message);
            return; 
        }
        else
        {
            saveSlot = slotToSaveTo;
            Save();
        }
    }

    /// <summary>
    /// Load a save from a slot.
    /// </summary>
    /// <param name="slotToLoadFrom"></param>
    public void LoadFromSlot(int slotToLoadFrom)
    {
        saveSlot = slotToLoadFrom;
        Load();
    }

    /// <summary>
    /// Called by asset tracking system. Not meant to be used.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void OnAssetImported()
    {
        ValidateGUID();
        OnAssetGenerated();
        int _saveSlot = saveSlot;
        saveSlot = -1;
        Save();
        saveSlot = _saveSlot;
    }

    /// <summary>
    /// Called by asset tracking system. Not meant to be used.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void OnWillDelete()
    {
#if UNITY_EDITOR
        DeleteAllSaveFiles();
#endif
        OnAssetDeletion();
    }

    /// <summary>
    /// Called by asset tracking system. Not meant to be used.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void OnWillSave()
    {
#if UNITY_EDITOR
        int _saveSlot = saveSlot;
        saveSlot = -1;
        Save();
        saveSlot = _saveSlot;
        OnAssetSaved();
#endif

    }

    /// <summary>
    /// Called after this asset has been imported/reimported.
    /// </summary>
    protected virtual void OnAssetGenerated()
    {

    }

    /// <summary>
    /// Called before this asset has been deleted.
    /// </summary>
    protected virtual void OnAssetDeletion()
    {

    }

    /// <summary>
    /// Called before the asset has been saved to Json
    /// </summary>
    protected virtual void OnAssetSaved()
    {

    }

    /// <summary>
    /// Called after the asset has been loaded from Json.
    /// </summary>
    protected virtual void OnAssetLoaded()
    {

    }


    internal virtual void Save()
    {
        if (serializedGuid == null || serializedGuid.Length ==0)
            return;
        if (!Directory.Exists(GetDirectory()))
            Directory.CreateDirectory(GetDirectory());
        OnAssetSaved();
        string contents = JsonUtility.ToJson(this);
        File.WriteAllText(GetFilePath(), contents);
    }

    internal virtual void Load()
    {
        if (File.Exists(GetFilePath()))
        {
            string content = File.ReadAllText(GetFilePath());
            if (!string.IsNullOrEmpty(content))
            {
                JsonUtility.FromJsonOverwrite(content, this);
                OnAssetLoaded();
            }
        }
    }

    internal void DeleteAllSaveFiles()
    {
        if (string.IsNullOrEmpty(serializedGuid))
            return;
        if (Directory.Exists(GetDirectory()))
            Directory.Delete(GetDirectory(), true);
    }

    internal protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private void EditorApplication_playModeStateChanged(UnityEditor.PlayModeStateChange obj)
    {
        switch (obj)
        {
            case UnityEditor.PlayModeStateChange.EnteredEditMode:
                break;
            case UnityEditor.PlayModeStateChange.ExitingEditMode:
                break;
            case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                int _saveSlot = saveSlot;
                saveSlot = -1;
                Save();
                saveSlot = _saveSlot;
                break;
            case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                int _saveSlot2 = saveSlot;
                saveSlot = -1;
                Load();
                saveSlot = _saveSlot2;
                break;
        }
    }
#endif

    internal protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
#endif
    }

    private string GetFileName()
    {
        return saveSlot.ToString() + FileExtension;
    }

    private string GetDirectory()
    {
        return Application.persistentDataPath + Path.DirectorySeparatorChar + FolderName  + Path.DirectorySeparatorChar + serializedGuid + Path.DirectorySeparatorChar;
    }

    private string GetFilePath()
    {
        return GetDirectory() + GetFileName();
    }

    private void ValidateGUID()
    {
#if UNITY_EDITOR

        if (UnityEditor.AssetDatabase.IsMainAsset(this))
        {
            string pathToMainAsset = UnityEditor.AssetDatabase.GetAssetPath(this);
            string mainAssetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(pathToMainAsset);
            if (mainAssetGuid != serializedGuid)
            {
                if (!string.IsNullOrEmpty(serializedGuid))
                {
                    Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(pathToMainAsset);
                    foreach (BaseScriptableObject baseScriptableObject in objects)
                    {
                        if (baseScriptableObject != this)
                        {
                            serializedGuid = System.Guid.NewGuid().ToString();
                        }
                    }
                }
                serializedGuid = UnityEditor.AssetDatabase.AssetPathToGUID(pathToMainAsset);
            }
        }
        else if (string.IsNullOrEmpty(serializedGuid))
        {
            serializedGuid = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}