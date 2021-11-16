using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AssetWeight
{
    public string assetName;
    public int weight;
}

public static class RandomAssetsUtils
{
    public static string GetRandom(List<AssetWeight> list)
    {
        if (list == null || list.Count == 0)
            return null;
        
        int totalNum = 0;
        foreach (var VARIABLE in list)
        {
            totalNum += VARIABLE.weight;
        }

        int r = Random.Range(0, totalNum);

        int t = 0;
        for (int i = 0; i < totalNum; i++)
        {
            t += list[i].weight;
            if (r < t)
            {
                return list[i].assetName;
            }
        }
        return list[0].assetName;
    }
}
