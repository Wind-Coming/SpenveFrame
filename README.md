# SpenveFrame
 Unity C#版本框架

## 第三方库  
1，dotween  老牌插件，用起来比较舒服，而且源码核心的那一段数学模型值得学习。  
2，FGUI（为什么会选择Fgui，这款UI开发思路比较偏程序，个人觉得它对逻辑与显示分离得比较开，而且上手也比较容易，美术同学可以自己搞定，功能开发起来比较快）。  
3，Odin 也算是老牌插件了，开发工具什么的都比较方便。  
4，Reporter，真机查看log比较方便。  

## 资源管理系统
这套资源管理的使用方式类似Addressable，但是没有那么复杂的代码，支持同步合异步，项目资源管理起来也比较方便。  
但需要注意的是所有需要动态加载的文件不能重名。（当然重名了会有提示）  
资源配置工具是用了unity原先的assetbundle插件进行修改的。  

### 配置  
资源工具->资源管理配置窗口  
<img width="186" alt="res_in" src="https://user-images.githubusercontent.com/18462688/142002483-34a79659-b77f-47f6-ab0b-f69df61368f2.png">  
目前资源管理系统并不依赖于assetbundle name，所以并不需要去配置这玩意儿。  
只需要将这些需要动态加载的文件所属文件夹配置进去即可，所以文件夹内的文件都会打包进bundle  
<img width="778" alt="build_config" src="https://user-images.githubusercontent.com/18462688/141997057-dc26846c-9360-47a1-ab2e-8309696a6178.png">  

### 打包资源  
如果是第一次打包备份一份md5，后续制作热更新可以自动检测并生成热更新包，运行时资源加载将与底层资源管理完全分离。
<img width="778" alt="buildbundle" src="https://user-images.githubusercontent.com/18462688/141997912-8a84da4e-7ee4-408a-ae5e-fb31a46dddb8.png">  

### 加载  
加载prefab：GameObject go = Pool.LoadGo(string address);  
卸载prefab: Pool.Unload(go);  

加载不需要实利化的资源（比如图片，声音等）：  
1，如果是常驻内存的资源可以使用：ResLoader.Global.LoadAsset<Type>(string address)  
2，需要是需要卸载的资源可以使用.   
 ResLoader rl = ResLoader.Get();  
 Type xxx = rl.LoadAsset<Type>(string address);  
 卸载时直接调用:ResLoader.Release(rl)即可，这句代码将会卸载所有rl加载过的资源。  
 
 有时候需要在Editor下调试Bundle，可以勾上模拟bundle模式  
 资源工具->模拟bundle模式  
 <img width="183" alt="simulate" src="https://user-images.githubusercontent.com/18462688/142002244-46713f0c-ee75-4d7a-abea-2a596f751648.png">  
 
 
## UI系统
 使用Fgui直接导出脚本，框架中将以Window的形式使用一个界面的Component  
 ```
 public class WindowName : UIWindowBase
 {
    private ComponentName uiCom;

    protected override void OnInit()
    {
        base.OnInit();

        packageName.BindAll();
        UIMgr.AddPackage("packageName");

        uiCom = ComponentName.CreateInstance();
        this.contentPane = uiCom.asCom;
        this.contentPane.MakeFullScreen();
    }
 }
 ```
 打开Window  
 UIMgr.Show<WindowName>();  
 隐藏Window  
 UIMgr.Hide<WindowName>();
 
 ##消息系统(另有带参数的接口)  
 监听  
 MsgSystem.Instance.AddListener("eventName", function);  
 移除监听  
 MsgSystem.Instance.RemoveListener("eventName", function);    
 Post消息  
 MsgSystem.Instance.PostMessage("eventName");  
 
 
