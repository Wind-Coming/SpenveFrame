using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class OrderSort : MonoBehaviour
{
    [MenuItem("Tools/美术工具/渲染Order层级自动排列")]

    static void SortOrder()
    {
        int index = -3;
        string[] _allPath = new string[] { "Assets/Art/Environment", "Assets/Art/Character/Block/Prefab" };
        for (int i = 0; i < _allPath.Length; i++)
        {
            var _allPrefabsPath = new string[] { _allPath[i] };
            var allpath = AssetDatabase.FindAssets("t: Prefab", _allPrefabsPath);
            var meshDict = new Dictionary<string, int>();

            var meshFilter = new Dictionary<string, int>();

            foreach (var path in allpath)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(path);
                if (prefabPath.IndexOf("Assets/Art/Environment/Level/Common") >=0)
                {
                    continue;
                }
                if (prefabPath.IndexOf("/Map")>=0||prefabPath.IndexOf("Assets/Art/Character/Block/Prefab")>=0)
                {
                    var pre = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    foreach (var mesh in pre.GetComponentsInChildren<MeshRenderer>())
                    {
                        mesh.sharedMaterial.enableInstancing = true;
                        var meshPath = AssetDatabase.GetAssetPath(mesh.GetComponent<MeshFilter>().sharedMesh);
                        var matName = mesh.sharedMaterial ? mesh.sharedMaterial.name : default;
                        if (string.IsNullOrEmpty(matName))
                        {
                            Debug.LogError(prefabPath, pre);
                            continue;
                        }
                        var key = matName + meshPath;
                        Debug.Log(key, pre);
                        if (!meshDict.TryGetValue(key, out var order))
                        {
                            if (matName == "NPC_Fan_WD_001_d_Mat")
                            {
                                mesh.sortingOrder = -2;
                                continue;
                            }

                            if (mesh.transform.tag == "Terrain")
                            {
                                mesh.sortingOrder = -1;
                                continue;
                            }
                            order = index--;
                            meshDict.Add(key, order);
                        }
                        mesh.sortingOrder = order;
                    }
                }

            }
        }
        AssetDatabase.SaveAssets();

    }
}
