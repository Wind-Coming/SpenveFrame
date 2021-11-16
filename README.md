# SpenveFrame
 Unity C#版本框架

## 第三方库  
1，dotween  老牌插件，用起来比较舒服，而且源码核心的那一段数学模型值得学习。  
2，FGUI（为什么会选择Fgui，这款UI开发思路比较偏程序，个人觉得它对逻辑与显示分离得比较开，而且上手也比较容易，美术同学可以自己搞定，功能开发起来比较快）。  
3，Odin 也算是老牌插件了，开发工具什么的都比较方便。  
4，Reporter，真机查看log比较方便。  

## 资源配置系统  
资源配置系统是用了unity原先的assetbundle插件进行修改的。  

### 配置
目前资源管理系统并不依赖于assetbundle name，所以并不需要去配置这玩意儿。  
只需要将这些需要动态加载的文件所属文件夹配置进去即可，所以文件夹内的文件都会打包进bundle  
<img width="778" alt="build_config" src="https://user-images.githubusercontent.com/18462688/141997057-dc26846c-9360-47a1-ab2e-8309696a6178.png">  

### 打包资源  
如果是第一次打包备份一份md5，后续制作热更新可以自动检测并生成热更新包，运行时资源加载将与底层资源管理完全分离。
<img width="778" alt="buildbundle" src="https://user-images.githubusercontent.com/18462688/141997912-8a84da4e-7ee4-408a-ae5e-fb31a46dddb8.png">
