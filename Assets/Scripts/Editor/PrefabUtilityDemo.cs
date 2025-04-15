using UnityEditor;
using UnityEngine;

public static class PrefabUtilityDemo
{
    // Creates a prefab at Assets/Prefabs/<name>.prefab and returns it
    public static GameObject CreateCubePrefab(string name)
    {
        var root = GameObject.CreatePrimitive(PrimitiveType.Cube);
        string path = $"Assets/Prefabs/{name}.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs");
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path, out bool success);
        Object.DestroyImmediate(root);
        if (!success) throw new System.Exception("Prefab save failed");
        AssetDatabase.Refresh();
        return prefab;
    }
}
