using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.OdinInspector;
using System.IO;

namespace ArtTool
{
    public class RelaceShaderWindow : OdinEditorWindow
    {
        [MenuItem("Tools/美术工具/材质球预制体依赖查找器")]
        private static void OpenWindow()
        {
            GetWindow<RelaceShaderWindow>().Show();
        }

        [TableList] public List<ShaderType> SomeTableData;

        public bool _isStandard = false;

        [Button(20)]
        private void RelaceShader()
        {
            if (_isStandard==false)
            {
                ShaderSoftEdgeUnlit();
            }
            else
            {
                ShaderSoftEdgeUnlit();
            }
        }

        public void ShaderSoftEdgeUnlit()
        {
            //Shader shader = Shader.Find(_rplacementShader);
            SomeTableData = new List<ShaderType>();
            string[] tempMaterialsPath = AssetDatabase.GetAllAssetPaths();
            List<Material> tempMaterials = new List<Material>();
            for (int i = 0; i < tempMaterialsPath.Length; i++)
            {
                string ext = Path.GetExtension(tempMaterialsPath[i]);
                if (ext != ".mat")
                {
                    continue;
                }

                var path = tempMaterialsPath[i];
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                var node = new ShaderType
                {
                    _material = mat,
                    _path = path,
                    _shader = mat.shader,
                };
                if (node._shader.name == "Standard"&&_isStandard==true)
                {
                    SomeTableData.Add(node);
                }
                else if (_isStandard==false)
                {
                    SomeTableData.Add(node);
                }
            }
            

            //if (tempMaterials.Count != 0)
            //{
            //    for (int i = 0; i < tempMaterials.Count; i++)
            //    {
            //        if (tempMaterials[i] == null)
            //        {
            //            continue;
            //        }
            //if (tempMaterials[i].shader.name == _rplacedmentShader)
            //{
            //    tempMaterials[i].shader = shader;
            //}
        }
    }


    public class ShowPrefabList : OdinEditorWindow
    {
        public static void OpenPreWindow(string path)
        {
            var win = GetWindow<ShowPrefabList>();
            win.Show();
            win.FindAllPrefabs(path);
        }

        [TableList] public List<PrefabType> _prefabs;

        public void FindAllPrefabs(string _ma)
        {
            _prefabs = new List<PrefabType>();

            string[] tempPrefabsPath = AssetDatabase.GetAllAssetPaths();

            for (int i = 0; i < tempPrefabsPath.Length; i++)
            {
                //EditorUtility.DisplayProgressBar("process", "checking..", (float)i / tempPrefabsPath.Length);
                string extPre = Path.GetExtension(tempPrefabsPath[i]);
                if (extPre != ".prefab")
                {
                    continue;
                }

                var depens = AssetDatabase.GetDependencies(tempPrefabsPath[i]);
                foreach (var g in depens)
                {
                    if (_ma == g)
                    {
                        var mat = AssetDatabase.LoadAssetAtPath<GameObject>(tempPrefabsPath[i]);
                        var _prefab = new PrefabType
                        {
                            _prefab = mat,
                            _prefabPath = tempPrefabsPath[i],
                        };
                        _prefabs.Add(_prefab);
                        break;
                    }
                }
            }

            //EditorUtility.DisplayCancelableProgressBar("process", "checking..", 1);
        }
    }

    public class PrefabType
    {
        public GameObject _prefab;

        public string _prefabPath;
    }

    public class ShaderType
    {
        public Material _material;

        public Shader _shader;

        public string _path;

        [ButtonGroup("找到材质球关联的预制体")]
        public void FindPrefabsButton()
        {
            ShowPrefabList.OpenPreWindow(_path);
        }
    }
}