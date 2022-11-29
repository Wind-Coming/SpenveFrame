using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public int num = 50;
    public float range = 10;
    public Material[] mats;
    
    [ContextMenu("Generate")]
    void Generate()
    {
        for (int i = 0; i < num; i++)
        {
            GameObject g = new GameObject(i.ToString());
            g.transform.position = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
            g.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
            g.AddComponent<MeshRenderer>().sharedMaterial = mats[Random.Range(0, mats.Length)];
        }
    }
}
