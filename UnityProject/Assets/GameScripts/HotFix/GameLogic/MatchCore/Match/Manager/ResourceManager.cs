using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace jc
{
    //资源类型
    public enum ResType
    {
        RT_LOG, //日志
        RT_CONFIG, //配置
        RT_SCRIPT, //脚本
        RT_SPRITE, //精灵
        RT_PREFAB, //预设
        RT_MATERIAL, //材质
        RT_AUDIO, //音频
        RT_VIDEO, //视频
        RT_STAGE //? Stage 配置
    }

    //资源类（单例）
    public sealed class ResourceManager : GameBase.Singleton<ResourceManager>
    {
        /** 属性变量 **/
        //资源路径映射表<资源类型、资源路径>
        private Dictionary<ResType, string> m_mapResPath = new Dictionary<ResType, string>();

        /** 构造函数 **/
        public ResourceManager()
        {
            this.m_mapResPath[ResType.RT_CONFIG] = "Assets/Resources";
            this.m_mapResPath[ResType.RT_SCRIPT] = "Assets/Scripts";
            this.m_mapResPath[ResType.RT_SPRITE] = "Assets/Sprites";
            this.m_mapResPath[ResType.RT_PREFAB] = "Assets/Prefabs";
            this.m_mapResPath[ResType.RT_MATERIAL] = "Assets/Material";
            this.m_mapResPath[ResType.RT_AUDIO] = "Assets/Medias/Audio";
            this.m_mapResPath[ResType.RT_VIDEO] = "Assets/Medias/Video";
            this.m_mapResPath[ResType.RT_STAGE] = "level";
        }

        /** 公有函数 **/
        /*  
         * 描  述：设置资源路径
         * 参  数：资源类型、路径
         * 返回值：无
         */
        public void SetResPath(ResType type, string path)
        {
            this.m_mapResPath[type] = path;
        }

        /*  
         * 描  述：获取资源路径
         * 参  数：资源类型
         * 返回值：资源路径
         */
        public string GetResPath(ResType type)
        {
            if (this.m_mapResPath.ContainsKey(type))
            {
                return this.m_mapResPath[type];
            }

            return string.Empty;
        }

        /*  
         * 描  述：加载精灵资源(单个)
         * 参  数：精灵名(相对路径，不带后缀名.png)
         * 返回值：无
         */
        public Sprite LoadSprite(string name, Action<Sprite> callback)
        {
            return null;
            // UnityEngine.Profiling.Profiler.BeginSample("LoadSprite");
            // string resPath = this.m_mapResPath[ResType.RT_SPRITE] + "/" + name + ".png";
            // Sprite objSprite = AssetsManager.Load<Sprite>(resPath, callback, false);
            // if (null == objSprite)
            // {
            //     Debug.LogError(string.Format("ResourceManager - LoadSprite - Load Sprite \"{0}\" Failed!", resPath));
            // }
            // UnityEngine.Profiling.Profiler.EndSample();
            // return objSprite;
        }

        /*  
         * 描  述：加载预设资源
         * 参  数：预设名(相对路径，不带后缀名.prefab)
         * 返回值：无
         */
        public GameObject LoadPrefab(string name, Action<GameObject> callback = null, bool isAsync = true)
        {
            // UnityEngine.Profiling.Profiler.BeginSample("LoadPrefab");
            // string resPath = this.m_mapResPath[ResType.RT_PREFAB] + "/" + name + ".prefab";
            // GameObject goInstance = null;
            // Action<GameObject> pCallback = (GameObject obj) =>
            // {
            //     goInstance = (GameObject) GameObject.Instantiate(obj);
            //     if (null == goInstance)
            //     {
            //         LogUtil.AddLog("resource", string.Format("ResourceManager - LoadPrefab - Load Prefab \"{0}\" Failed!", resPath));
            //     }
            //     if (callback != null)
            //     {
            //         callback(goInstance);
            //     }
            //     UnityEngine.Profiling.Profiler.EndSample();
            // };
            // AssetsManager.Load<GameObject>(resPath, pCallback, isAsync);
            // return goInstance;
            return null;
        }

        /*  
         * 描  述：加载材质资源
         * 参  数：材质名(相对路径，不带后缀名.mat)
         * 返回值：无
         */
        public Material LoadMaterial(string name)
        {
            // UnityEngine.Profiling.Profiler.BeginSample("LoadMaterial");
            // string resPath = this.m_mapResPath[ResType.RT_MATERIAL] + "/" + name + ".mat";
            // Material objMaterial = AssetsManager.Load<Material>(resPath);
            // if (null == objMaterial)
            // {
            //     LogUtil.AddLog("resource", string.Format("ResourceManager - LoadMaterial - Load Material \"{0}\" Failed!", resPath));
            // }

            // UnityEngine.Profiling.Profiler.EndSample();
            // return objMaterial;
            return null;
        }

        /*  
         * 描  述：加载音频资源
         * 参  数：音频名(相对路径，不带后缀名.ogg)
         * 返回值：无
         */
        public AudioClip LoadAudio(string name)
        {
            return null;
            // UnityEngine.Profiling.Profiler.BeginSample("LoadAudio");
            // string resPath = this.m_mapResPath[ResType.RT_AUDIO] + "/" + name + ".ogg";
            // AudioClip objAudio = AssetsManager.Load<AudioClip>(resPath);
            // if (null == objAudio)
            // {
            //     LogUtil.AddLog("resource", string.Format("ResourceManager - LoadAudio - Load Audio \"{0}\" Failed!", resPath));
            // }

            // UnityEngine.Profiling.Profiler.EndSample();
            // return objAudio;
        }
        /*  
         * 描  述：加载关卡资源
         * 参  数：关卡名(相对路径，不带后缀名.xml)
         * 返回值：无
         */
        public ENate.StageConfig LoadStage(string name)
        {
            return null;
            // UnityEngine.Profiling.Profiler.BeginSample("LoadStage");
            // string resPath = this.m_mapResPath[ResType.RT_STAGE] + "/" + name;
            // ResourceRequest element_config = AssetsManager.LoadJsonResource(resPath);
            // TextAsset tTextAsset = element_config.asset as TextAsset;
            // ENate.StageConfig tStageConfig = null;
            // if (tTextAsset != null)
            // {
            //     tStageConfig = new ENate.StageConfig();
            //     tStageConfig.Load(tTextAsset.text);
            // }
            // UnityEngine.Profiling.Profiler.EndSample();
            // return tStageConfig;
        }

        /** 操作属性变量 **/
        public Dictionary<ResType, string> _ResPathMap
        {
            get { return this.m_mapResPath; }
        }
    }
}