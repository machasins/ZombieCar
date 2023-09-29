using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get notified after the asset is imported.
/// </summary>
public interface IOnPostProcessImportAssetCallback
{
    void OnAssetImported();
}

/// <summary>
/// Get notified before the asset is deleted.
/// </summary>
public interface IOnWillDeleteAssetCallback
{
    void OnWillDelete();
}

/// <summary>
/// Get notified before the asset is deleted.
/// </summary>
public interface IOnWillSaveAssetCallback
{
    void OnWillSave();
}

/// <summary>
/// Get notified when this asset is moved
/// </summary>
public interface IOnPostProcessMoveAssetCallback
{
    void OnAssetMoved();
}

