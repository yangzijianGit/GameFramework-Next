using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace jc
{
    //公共类（无实例）
    public class CommonUtils
    {
        //-------------------------------------------------------------
        /*  
         * 描  述：设置对象皮肤(静态)
         * 参  数：游戏对象，皮肤文件、子块[默认]
         * 返回值：无
         */
        public static void SetNormalSkin(GameObject obj, string skinFile, bool IsSetNative = true, Action<Sprite> callback = null)
        {

            skinFile = skinFile.Replace("\\","/");
            if (obj.GetComponent<Image>() == null)
            {
                obj.AddComponent<Image>();
            }
            Image objImage = obj.GetComponent<Image>();
            if (null == objImage) {
                Debug.LogError( string.Format("CommonUtils - SetNormalSkin - Not Found Image Component! \"{0}\"", obj.name));
                return;
            }

            // AssetsManager.Load<Sprite>("Assets/_Sprites/" + skinFile+".png",(Sprite sp)=> {
            //     objImage.sprite = sp;
            //     if (IsSetNative == true)
            //     {
            //         objImage.SetNativeSize();
            //     }
            //     if(callback != null)
            //     {
            //         callback(sp);
            //     }
            // },false);
        }

        /*  
         * 描  述：设置对象皮肤透明度(静态)
         * 参  数：游戏对象，透明度(0-1)
         * 返回值：无
         */
        public static void SetSkinAlpha(GameObject obj, float alpha)
        {
            Image objImage = obj.GetComponent<Image>();
            if (null == objImage) {
                Debug.LogError( string.Format("CommonUtils - SetSkinAlpha - Not Found Image Component! \"{0}\"", obj.name));
                return;
            }

            objImage.color = new Color(objImage.color.r, objImage.color.g, objImage.color.b, alpha);
        }

        /*  
         * 描  述：修改显示隐藏
         * 参  数：游戏对象，显示隐藏
         * 返回值：无
         */
        public static void SetObjActive(GameObject obj, bool isActive, bool IsAtTop = false)
        {
            if (obj == null)
            {
                Debug.LogError( string.Format("CommonUtils - SetObjActive - Not Found obj！ \"{0}\"", obj.name));
            }
            else
            {
                if (obj.activeSelf != isActive)
                {
                    obj.SetActive(isActive);
                    if (IsAtTop == true)
                    {
                        obj.transform.SetAsLastSibling();
                    }
                }
            }
        }

        public static void SetImageMaskMaterial(GameObject obj)
        {
            if(obj != null)
            {
                // AssetsManager.Load<Material>("Assets/_Prefabs/Effect/Material/ui_buildingOntLine.mat",(Material ma)=> {
                //     ma.shader = Resources.Load<Shader>("UI_Base");
                //     obj.GetComponent<Image>().material = ma;
                // });
            }
        }

        public static void SetImageNoneMaskMaterial(GameObject obj)
        {
            if(obj != null)
            {
                obj.GetComponent<Image>().material = null;
            }
        }

        public static void SetImageFillAmount(GameObject obj ,float value)
        {
            Image objImage = obj.GetComponent<Image>();
            if (objImage == null)
            {
                Debug.LogError( string.Format("CommonUtils - SetImageFillAmount - Not Found obj！ \"{0}\"", obj.name));
            }
            objImage.fillAmount = value;
        }

        /// <summary>
        /// 获得幸福星星，需要根据在线时长计算
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetHappinessReward(int value)
        {
            // int m_anti = (int)PlayerData.Instance.Anti;
            // if (m_anti >= 3 && m_anti < 5)
            // {
            //     value = (int)(value * 0.5f);
            // }
            // else if (m_anti >= 5)
            // {
            //     value = 0;
            // }
            // return value;
            return 0;
        }

        public static string GetGold(long gold)
        {
            if (gold > 1000)
            {
                return ((long)(gold / 1000)).ToString() + "k";
            }
            else
            {
                return gold.ToString();
            }
        }

        //-------------------------------------------------------------

        public static void SetTextColor(GameObject obj, string color)
        {
            Text objText = obj.GetComponent<Text>();
            if (objText != null)
            {
                objText.text = "<color=#02"+ color+">" + objText.text + "</color>";
            }
        }
        
        public static void SetAnimation(string t_asset_path,System.Action<GameObject> callback)
        {
            // AssetsManager.Load(t_asset_path, callback);
        }
        /*  
         * 描  述：通过key设置多语言文本(静态)
         * 参  数：游戏对象，文本key，是否直接是文本[默认]
         * 返回值：无
         */
        public static void SetKeyText(GameObject obj, string key, bool isText = false)
        {
            // TMPro.TextMeshProUGUI objTM = obj.GetComponent<TMPro.TextMeshProUGUI>();
            // if (objTM != null) {
            //     if (isText) {
            //         objTM.text = key;
            //     }
            //     else {
            //         objTM.text = LanguageManager.Instance.GetLanguage(key);
            //     }
            //     return;
            // }

            // Text objText = obj.GetComponent<Text>();
            // if (objText != null) {
            //     if (isText) {
            //         objText.text = key;
            //     }
            //     else {
            //         objText.text = LanguageManager.Instance.GetLanguage(key);
            //     }
            //     return;
            // }

            Debug.LogError( string.Format("CommonUtils - SetKeyText - Not Found Text Or TextMeshProUGUI Component! \"{0}\"", obj.name));
        }

        /// <summary>
        /// 获得一组随机数[min][max]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<int> GetRandom(int min, int max, int count)
        {
            List<int> rArray = new List<int>();
            int range = max - min + 1;

            if (range < count)
            {
                for (int i = 0; i < count; i++)
                {
                    rArray.Add(i % range + min);
                }
            }
            else if (range == count)
            {
                for (int i = min; i <= max; i++)
                {
                    rArray.Add(i);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    System.Random rd = new System.Random(i);
                    int tIndex = rd.Next(min, max+1);
                    rArray.Add(tIndex);
                }
            }
            return rArray;
        }

        //-------------------------------------------------------------
        /*  
         * 描  述：字符串转位置(静态)
         * 参  数：字符串(逗号分割)
         * 返回值：位置
         */
        public static Vector2 StrToPos(string str)
        {
            string [] parts = str.Split(',');
            if(parts.Length == 2)
            {
                return new Vector2(int.Parse(parts[0]), int.Parse(parts[1]));
            }

            return Vector2.zero;
        }

        //-------------------------------------------------------------
        /*  
         * 描  述：创建UI对象(静态)
         * 参  数：预设脚本名、父节点[默认]
         * 返回值：游戏对象
         */
        public static GameObject CreateUIObject(string scriptName, GameObject parent = null)
        {
            // if (!UIManager.Instance._UIPrefabMap.ContainsKey(scriptName)) {
            //     Debug.LogError( string.Format("CommonUtils - CreateUIObject - Not Found UI Script! \"{0}\"", scriptName));
            //     return null;
            // }

            // var objPrefab = UIManager.Instance._UIPrefabMap[scriptName];
            // GameObject go = ResourceManager.Instance.LoadPrefab(objPrefab.strPrefabName);
            // if (go != null) {
            //     go.name = objPrefab.strPrefabName;
                
            //     if (parent != null) {
            //         go.transform.SetParent(parent.transform);
            //     }
            //     else {
            //         go.transform.SetParent(ViewManager.viewMap[objPrefab.strUIName].ViewRoot.transform);
            //         go.SetActive(false);
            //     }
            // }

            // return go;
            return null;
        }

        //-------------------------------------------------------------
        /// <summary>
        /// 根据N,S,R,SSR获取资源路径
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetFrameFromQuality(string level)
        {
            string path;
            if(level == "1")
            {
                path = "UI/UI_Public/itemBg_Q1";
            }
            else if(level == "2")
            {
                path = "UI/UI_Public/itemBg_Q2";
            }
            else if(level == "3")
            {
                path = "UI/UI_Public/itemBg_Q3";
            }
            else if(level == "4")
            {
                path = "UI/UI_Public/itemBg_Q4";
            }
            else
            {
                path = "none";
            }
            return path;
        }

        /// <summary>
        /// 根据品质，获得带颜色的资源地址
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static string GetLetterFromQuality(string quality)
        {
            string path;
            if (quality == "N" || quality == "1")
            {
                path = "UI/UI_Public/item_Q1";
            }
            else if (quality == "R" || quality == "2")
            {
                path = "UI/UI_Public/item_Q2";
            }
            else if (quality == "SR" || quality == "3")
            {
                path = "UI/UI_Public/item_Q3";
            }
            else if(quality == "SSR" || quality == "4")
            {
                path = "UI/UI_Public/item_Q4";
            }
            else
            {
                path = "UI/UI_Public/item_Q1";
            }
            return path;
        }

        public static string GetCurrencyImgPath(string type)
        {
            // string path = TableManager.currencyDic[type].pic;
            // return path;
            return "";
        }

        public static string GetTime(string timeStr)
        {
            string result = "";
            string[] strs = timeStr.Split(' ');
            string monsth = strs[4];
            string day = "";
            if(strs[3].IndexOf("-")==-1)
            {
                day = strs[3];
            }
            else
            {
                day = strs[3].Split('-')[1];
            }
            // result =  LanguageManager.Instance.GetLanguage("Time_16",new string[] { monsth }) +  LanguageManager.Instance.GetLanguage("Time_17",new string[] { day});
            return result;
        }

        public static bool IsBetweenTime(string timeStr)
        {
            int second = DateTime.Now.Second;
            int minute = DateTime.Now.Minute;
            int hour = DateTime.Now.Hour;
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            DayOfWeek week = DateTime.Now.DayOfWeek;
            int dayWeek = (int)FormatWeek(week);
            string[] strs = timeStr.Split(' ');
            if (formatWeek(strs[6], dayWeek) == false)
            {
                return false;
            }
            if (formatDate(strs[5], year) == false)
            {
                return false;
            }
            if (formatDate(strs[4], month) == false)
            {
                return false;
            }
            if (formatDate(strs[3], day) == false)
            {
                return false;
            }
            if (formatDate(strs[2], hour) == false)
            {
                return false;
            }
            if (formatDate(strs[1], minute) == false)
            {
                return false;
            }
            if (formatDate(strs[0], second) == false)
            {
                return false;
            }

            return true;
        }
        private static bool formatWeek(string strs, int nowTime)
        {
            bool yearbol = true;
            if (strs != "?" || strs != "*")
            {
                if (strs.IndexOf("-") != -1)
                {
                    int value1 = (int)((Week)Enum.Parse(typeof(Week), strs.Split('-')[0], true));
                    int value2 = (int)((Week)Enum.Parse(typeof(Week), strs.Split('-')[1], true));
                    if (nowTime <= value2 && nowTime > value1)
                    {
                        yearbol = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(strs == "*")
                {
                    yearbol = true;
                }
                else
                {
                    if (nowTime != (int)((Week)Enum.Parse(typeof(Week), strs, true)))
                    {
                        return false;
                    }
                }
            }
            return yearbol;
        }
        private static bool formatDate(string strs, int nowTime)
        {
            bool yearbol = true;
            if (strs == "?" || strs == "*")
                return yearbol;
            if (strs !="*")
            {
                if (strs.IndexOf("-") != -1)
                {
                    int value1 = int.Parse(strs.Split('-')[0]);
                    int value2 = int.Parse(strs.Split('-')[1]);
                    if (nowTime <= value2 && nowTime > value1)
                    {
                        yearbol = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (nowTime != int.Parse(strs))
                    {
                        return false;
                    }
                }
            }
            return yearbol;
        }

        private static Week FormatWeek(DayOfWeek day)
        {
            Week targetDay;
            if(day == DayOfWeek.Sunday)
            {
                targetDay = Week.SUN;
            }
            if (day == DayOfWeek.Monday)
            {
                targetDay = Week.MON;
            }
            if (day == DayOfWeek.Tuesday)
            {
                targetDay = Week.TUE;
            }
            if (day == DayOfWeek.Wednesday)
            {
                targetDay = Week.WED;
            }
            if (day == DayOfWeek.Thursday)
            {
                targetDay = Week.THU;
            }
            if (day == DayOfWeek.Friday)
            {
                targetDay = Week.FRI;
            }
            else
            {
                targetDay = Week.SAT;
            }
            return targetDay;
        }

        /// <summary>
        /// 发送移动摄像机的事件
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="IsReturn"></param>
        public static void SendMoveCamera(Vector3 pos, bool IsReturn)
        {
            // CET_move_camera eve = new CET_move_camera();
            // eve.vec = pos;
            // eve.IsReturn = IsReturn;
            // EventManager.Instance.NoticeEvent((int)CommonEventType.ET_move_camera, eve);
        }

        /// <summary>
        /// 发送移动摄像机的事件
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="IsReturn"></param>
        public static void SendMoveCamera(string pos, bool IsReturn)
        {
            SendMoveCamera(StrToPos(pos), IsReturn);
        }

        /// <summary>
        /// 获取text组件内容.
        /// </summary>
        public static string GetTextKey(GameObject obj)
        {
            string str = string.Empty;
            Text objText = obj.GetComponent<Text>();
            if (objText != null)
            {
                str = objText.text;
            }
            return str;
        }

        /// <summary>
        /// 设置Image灰度
        /// </summary>
        /// <param name="go">image</param>
        /// <param name="saturation">0-1 灰度-原色</param>
        public static void SetImageGray(GameObject go, float saturation)
        {
            // if (go == null)
            // {
            //     Debug.LogError( "CommonUtils - SetImageGray - gameobject == null");
            //     return;
            // }
            // if (go.GetComponent<Image>() == null)
            // {
            //     Debug.LogError( "CommonUtils - SetImageGray - Image == null");
            //     return;
            // }
            // Image image = go.GetComponent<Image>();
            // Material mat = AssetsManager.Load<Material>("Assets/_Prefabs/Effect/Material/UIGrayScale.mat");
            // if (mat == null)
            //     return;
            // image.material = mat;
            // saturation = Mathf.Clamp(saturation, 0f, 1f);
            // image.material.SetFloat("_Saturation", saturation);
        }

        public static void SetIcon(GameObject image, string type, string id, string subId="", bool IsSetNative = true)
        {
            // if(type == RewardType.appearance)
            // {
            //     SetNormalSkin(image, TableManager.GetBuildImgPath(subId, id), IsSetNative);
            // }else if(type == RewardType.item)
            // {
            //     SetNormalSkin(image, TableManager.itemDic[id].skin, IsSetNative);
            // }else if(type == RewardType.good)
            // {
            //     SetNormalSkin(image, TableManager.goodsDic[id].imgIcon, IsSetNative);
            // }else if(type == RewardType.postcard)
            // {
            //     SetNormalSkin(image, TableManager.postcardDic[id].imgIcon, IsSetNative);
            // }else if(type == RewardType.part)
            // {
            //     SetNormalSkin(image, TableManager.partDic[id].pic, IsSetNative);
            // }else if(type == RewardType.clothes)
            // {
            //     SetNormalSkin(image, TableManager.clothesDict[id].imgIcon, IsSetNative);
            // }
        }

        public static int GetPlotCondition(string str)
        {
            int var = -1;
            switch(str)
            {
                case "LevelEnd":
                    var = 0;
                    break;
                case "CompleteQuest":
                    var = 1;
                    break;
            }
            return var;
        }
        /// <summary>
        /// 设置按钮是否选中,用于TOGGLE类型的按钮，要求按钮下选中项需命名为selectObj,非选中为Image
        /// </summary>
        /// <param name="btn">按钮</param>
        /// <param name="isShow">是否选中</param>
        public static void SetBtnState(GameObject btn, bool isShow)
        {
            Transform tr = btn.transform.Find("selectObj");
            Transform normalTr = btn.transform.Find("Image");
            if (tr == null)
            {
                Debug.LogError( "设置TOGGLE类型的按钮，要求按钮下选中项需命名为selectObj,但未找到selectObj");
            }
            else
            {
                tr.gameObject.SetActive(isShow);
                if (normalTr == null)
                {
                    Debug.LogError( "未选中的图片命名需为Image,且此层需只有一个Image的命名");
                }
                else
                {
                    normalTr.gameObject.SetActive(!isShow);
                }
            }
        }

        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="go">要设置的物体</param>
        /// <param name="parent">父节点</param>
        public static void SetParent(GameObject go, Transform parent)
        {
            go.transform.SetParent(parent);
            RectTransform uiRect = go.GetComponent<RectTransform>();
            uiRect.localScale = Vector3.one;
            uiRect.localPosition = Vector3.zero;
            uiRect.sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
        }
    }   
}