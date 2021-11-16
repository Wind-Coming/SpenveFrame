using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RenderQRepeatSearch : MonoBehaviour
{
    [MenuItem("Assets/工具/RenderQ重复查找工具")]

    static void FindSameRenderQ()
    {
        Object[] _allMaterail = GetSelectedMaterail();
        List<Material> materials=new List<Material>();

        foreach (Material material in _allMaterail)
        {
            materials.Add(material);
        }
        for (int i = 0; i < materials.Count; i++)
        {
            for (int j = i+1; j < materials.Count; j++)
            {
                if (materials[i].renderQueue==materials[j].renderQueue)
                {
                    Debug.Log(materials[j].name+"+"+materials[i].name,materials[j]);
                }
            }
        }
    }

    static Object[] GetSelectedMaterail()
    {
        return Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
    }
}
