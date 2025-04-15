using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PrefabUtility_EditMode
{
    const string PrefabName = "TestCube";

    [Test]
    public void CreatesPrefabAndRegistersInAssetDatabase()
    {
        // 1. Act
        GameObject prefab = PrefabUtilityDemo.CreateCubePrefab(PrefabName);

        // 2. Assert
        string expectedPath = $"Assets/Prefabs/{PrefabName}.prefab";
        Assert.IsTrue(System.IO.File.Exists(expectedPath), "Prefab file not found on disk");
        Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<GameObject>(expectedPath),
            "Prefab not registered in AssetDatabase");

        // 3. Clean‑up
        AssetDatabase.DeleteAsset(expectedPath);
    }
}
