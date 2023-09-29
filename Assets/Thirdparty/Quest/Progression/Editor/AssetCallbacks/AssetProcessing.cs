using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

internal class PostProcessImportAndMove : UnityEditor.AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < importedAssets.Length; i++)
        {
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i]);
            if (t != null? !t.GetInterfaces().Contains(typeof(IOnPostProcessImportAssetCallback)):true)
                continue;
            object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(importedAssets[i]);
            if (assets != null)
                for (int ii = 0; ii < assets.Length; ii++)
                {
                    IOnPostProcessImportAssetCallback postprocess = (IOnPostProcessImportAssetCallback) assets[ii];
                    if (postprocess != null)
                        postprocess.OnAssetImported();
                }
        }
        for (int i = 0; i < movedAssets.Length; i++)
        {
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(movedAssets[i]);
            if (t != null ? !t.GetInterfaces().Contains(typeof(IOnPostProcessMoveAssetCallback)) : true)
                continue;
            object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(importedAssets[i]);
            if (assets != null)
                for (int ii = 0; ii < assets.Length; ii++)
                {
                    IOnPostProcessMoveAssetCallback postprocess = (IOnPostProcessMoveAssetCallback)assets[ii];
                    if (postprocess != null)
                        postprocess.OnAssetMoved();
                }
        }
    } 
}

internal class PreProcessAssetDeletion : UnityEditor.AssetModificationProcessor
{
    private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetToDelete, UnityEditor.RemoveAssetOptions removeAssetOptions)
    {
        System.Type t = AssetDatabase.GetMainAssetTypeAtPath(assetToDelete);
        if (t != null ? !t.GetInterfaces().Contains(typeof(IOnWillDeleteAssetCallback)) : true)
            return UnityEditor.AssetDeleteResult.DidNotDelete;
        object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetToDelete);
        if (assets != null)
            for (int ii = 0; ii < assets.Length; ii++)
            {
                IOnWillDeleteAssetCallback onWillDelete = (IOnWillDeleteAssetCallback)assets[ii];
                if (onWillDelete != null)
                    onWillDelete.OnWillDelete();
            }
        return UnityEditor.AssetDeleteResult.DidNotDelete;
    }
}

internal class PreProcessAssetSave : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(paths[i]);
            if (t != null ? !t.GetInterfaces().Contains(typeof(IOnWillSaveAssetCallback)) : true)
                continue;
            object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(paths[i]);
            if (assets != null)
                for (int ii = 0; ii < assets.Length; ii++)
                {
                    IOnWillSaveAssetCallback postprocess = (IOnWillSaveAssetCallback)assets[ii];
                    if (postprocess != null)
                        postprocess.OnWillSave();
                }
        }
        return paths;
    }
}
#endif