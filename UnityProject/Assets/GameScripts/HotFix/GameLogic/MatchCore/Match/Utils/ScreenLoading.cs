// using System;
// using System.Collections;
// using jc;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// public class ScreenLoading : View
// {
//     private static ScreenLoading mInStance;
//     private RectTransform canvas;
//     private Transform tiplayer;

//     public ScreenLoading()
//     {
//         this.Name = "ScreenLoading";
//         this.PrefabPath = "Assets/_Prefabs/UI/UI_Loading.prefab";
//     }

//     public static ScreenLoading Instance
//     {
//         get
//         {
//             if (mInStance == null)
//             {
//                 mInStance = new ScreenLoading();
//             }
//             return mInStance;
//         }
//     }

//     public void Init()
//     {
//         Action<View> onLoad = (view) =>
//         {
//             canvas = GameObject.Find("UI").GetComponent<RectTransform>();
//             ViewRoot.SetActive(false);
//         };

//         tiplayer = GameObject.Find("UI/TipLayer").transform;
//         Load(onLoad);
//     }

//     public void Show(Action onComplete)
//     {
//         Action<string> postShow = (name) =>
//         {
//              onComplete();
//         };
//         Show(tiplayer, postShow);
//     }

//     public void Hide(Action onComplete=null)
//     {
//         IsShowing = false;
//         if (ViewRoot != null)
//         {
//             ViewRoot.SetActive(false);
//             if (onComplete != null)
//             {
//                 onComplete();
//             }
//         }
//     }
// }