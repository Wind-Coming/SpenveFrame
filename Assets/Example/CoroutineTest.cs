using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Spenve;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoroutineTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CoroutineSystem.Instance.StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        // int a = 5;
        // int b = 7;
        yield return null;
        //int c = a + b;
    }

    private static int num = 1024 * 1024;
    
    [MenuItem("Test/txt")]
    static void Testtxt()
    {
        float t = Time.realtimeSinceStartup;

        string path = Application.dataPath + "/MyInfo.txt";
        // 文件流创建一个文本文件
        FileStream file = new FileStream(path, FileMode.Create);

        byte[] bts = new byte[num * 2];
        for (int i = 0; i < num; i++)
        {
            bts[i * 2] = (byte)(128 + i % 128);
            bts[i * 2 + 1] = 10;
        }
        
        // 文件写入数据流
        file.Write(bts, 0, bts.Length);
        if (file != null)
        {
            //清空缓存
            file.Flush();
            // 关闭流
            file.Close();
            //销毁资源
            file.Dispose();
        }
        
        Debug.Log(Time.realtimeSinceStartup - t);
    }

    // [StructLayout(LayoutKind.Explicit)]
    // struct Converter
    // {
    //     [FieldOffset(0)]
    //     public byte[] mybytes;
    //     [FieldOffset(0)]
    //     public int[] mycols;
    // }
    //
    [StructLayout(LayoutKind.Explicit)]
    struct ConverterTT
    {
        [FieldOffset(0)]
        public byte[] a;
        [FieldOffset(0)]
        public uint[] b;
    }
    
    static Color32[] Convert(byte[] input){
        //This function is called in order to pass in a byte array and convert it to a Color32 array.
        //The 'Converter' type defined below in the struct, is basically just a custom type with a byte array field
        //Except that also in that structure, the [FieldOffset] is used to say at what memory location to locate the data for a given field
        //Then within that custom type, we have a Color32 field (at this point unused), which happens to map to the same memory location as the byte array, so they share memory
        //So if we simply set the byte array to that custom type's byte array, the type's Color32 array will also be pointing to that same memory reference, with no 'copy' of any memory bytes
        //So the reference in it to the byte array points to the same mybytearray[] memory as the Color32[] array.
        //We then simply return the Color32[] array and hey presto, the byte array has been interpreted as a Color32 array with almost zero overhead.
        Converter converter = new Converter();
        converter.mybytes = input;
        return converter.mycols;
    }
    [StructLayout(LayoutKind.Explicit)]
    struct Converter
    {
        [FieldOffset(0)]
        public byte[] mybytes;
        [FieldOffset(0)]
        public Color32[] mycols;
        [FieldOffset(1)] 
        public ushort[] mshort;
    }
    
    [MenuItem("Test/Readtxt")]
    static void ReadTesttxt()
    {
        // byte[] mybytearray = new byte[1000];
        // object inputObj = mybytearray;
        // mybytearray[0]=10;
        // mybytearray[1]=20;
        // mybytearray[2]=30;
        // mybytearray[3]=0; 
        //
        // mybytearray[999]=40;
        //
        // Color32[] mycolors = Convert(mybytearray);    //'Cast' byte array as Color32 array
        // Debug.Log(mycolors[0]);    //Should show 10,20,30,40 without having ever copied any data to a separate Color32 array
        // Debug.Log(mycolors[249]);    //Should show 10,20,30,40 without having ever copied any data to a separate Color32 array
        //
        // ConverterTT a = new ConverterTT();
        //
        // a.a = new byte[4];
        // a.a[0] = 1;
        // a.a[1] = 0;
        // a.a[2] = 0;
        // a.a[3] = 0;
        //
        // uint[] b = a.b;
        //
        // Debug.Log(b[0]);
        //
        // // Rect r = new Rect();
        // // r.left = 256;
        // return;
        float t = Time.realtimeSinceStartup;
        string path = Application.dataPath + "/MyInfo.txt";
        //逐行读取返回的为数组数据
        //string[] strs = File.ReadAllLines(path);
        
        byte[] tt = File.ReadAllBytes(path);
        
        // Converter converter = new Converter();
        // converter.mybytes = tt;
        
        // for(int i = 35; i < strs.Length; i++)
        // {
        //     Debug.Log((uint)strs[i][0]);
        //     // string[] str = item.Split(" ");
        //     // int i = int.Parse(str[0]);
        //     // int i1 = int.Parse(str[1]);
        //     //
        //     // int i2 = int.Parse(str[2]);
        //     // int i3 = int.Parse(str[3]);
        //     //
        // }
        
        // byte[] tt = File.ReadAllBytes(path);
        //
        // Converter converter = new Converter();
        // converter.mybytes = tt;
        //
        // for (int i = 0; i < converter.mycols.Length; i++)
        // {
        //     //Debug.Log(converter.mycols[i]);
        // }
        Debug.Log(Time.realtimeSinceStartup - t);
    }
}
