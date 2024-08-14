using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AssetUtil {
    private GameObject _letterimage;
    private GameObject _letter;
    private GameObject _starImage;
    private GameObject _cell;
    private GameObject _lineWord;
    private GameObject _levelButton;
    private GameObject _rubyFly;
    private GameObject _bonusWord;
    private GameObject _reward;
    private GameObject _achieveLine;
    private GameObject _rewardItem;

    private GameObject _eff_beijing1_huaban;
    private GameObject _eff_beijing3_huaban;
    private GameObject _eff_beijing5_pugongying;
    private GameObject _eff_beijing8_wu;
    private GameObject _eff_beijing_liuxing;
    private GameObject _eff_cell_glow;
    private GameObject _eff_beijing_rain;
    private GameObject _eff_additional_first;
    private GameObject _eff_additional_second;
    private GameObject _eff_additional_box;
    private GameObject _eff_GOOD_glow;
    private GameObject _eff_Video;

    private static AssetUtil mInStance;

    public static AssetUtil instance
    {
        get
        {
            if (mInStance == null)
            {
                mInStance = new AssetUtil();
            }
            return mInStance;
        }
    }

    public GameObject reward
    {
        get
        {
            if (_reward == null)
            {
                // _reward = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Home/Reward.prefab", null, false);
            }
            return _reward;
        }
    }

    public GameObject bonusWord
    {
        get
        {
            if(_bonusWord == null)
            {
                _bonusWord = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/ExtraWord/BonusWordText.prefab",null,false);
            }
            return _bonusWord;
        }
    }

    public GameObject cell
    {
        get
        {
            if (_cell == null)
            {
                _cell = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Main/Cell.prefab",null,false);
            }
            return _cell;
        }
    }

    public GameObject letter
    {
        get
        {
            if (_letter == null)
            {
                _letter = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/World/NewLetterText.prefab", null, false);
            }
            return _letter;
        }
    }
    
    public GameObject letterimage
    {
        get
        {
            if (_letterimage == null)
            {
                _letterimage = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Main/LetterText.prefab", null, false);
            }
            return _letterimage;
        }
    }

    public GameObject lineWord
    {
        get
        {
            if (_lineWord == null)
            {
                _lineWord = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Main/LineWord.prefab", null, false);
            }
            return _lineWord;
        }
    }

    public GameObject rubyFly
    {
        get
        {
            if (_rubyFly == null)
            {
                _rubyFly = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Main/RubyFly.prefab", null, false);
            }
            return _rubyFly;
        }
    }

    public GameObject levelButton
    {
        get
        {
            if (_levelButton == null)
            {
                _levelButton = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/World/NewLevelButton.prefab", null, false);
            }
            return _levelButton;
        }
    }

    public GameObject achieveLine
    {
        get
        {
            if (_achieveLine == null)
            {
                _achieveLine = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Quest/AchieveItem.prefab", null, false);
            }
            return _achieveLine;
        }
    }

    public GameObject rewardItem
    {
        get
        {
            if (_rewardItem == null)
            {
                _rewardItem =null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/_Prefabs/Activity/RewardItem.prefab", null,false);
            }
            return _rewardItem;
        }
    }

    public GameObject eff_beijing1_huaban
    {
        get
        {
            if (_eff_beijing1_huaban == null)
            {
                _eff_beijing1_huaban = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_beijing1_huaban.prefab", null, false);
            }
            return _eff_beijing1_huaban;
        }
    }
    public GameObject eff_beijing3_huaban
    {
        get
        {
            if (_eff_beijing3_huaban == null)
            {
                _eff_beijing3_huaban = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_beijing3_huaban.prefab", null, false);
            }
            return _eff_beijing3_huaban;
        }
    }
    public GameObject eff_beijing5_pugongying
    {
        get
        {
            if (_eff_beijing5_pugongying == null)
            {
                _eff_beijing5_pugongying = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_beijing5_pugongying.prefab", null, false);
            }
            return _eff_beijing5_pugongying;
        }
    }
    public GameObject eff_beijing_wu
    {
        get
        {
            if (_eff_beijing8_wu == null)
            {
                _eff_beijing8_wu = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_beijing8.15.18_wu.prefab", null, false);
            }
            return _eff_beijing8_wu;
        }
    }
    public GameObject eff_beijing_liuxing
    {
        get
        {
            if (_eff_beijing_liuxing == null)
            {
                _eff_beijing_liuxing = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_beijing19.28_liuxing.prefab", null, false);
            }
            return _eff_beijing_liuxing;
        }
    }
    public GameObject eff_cell_glow
    {
        get
        {
            if (_eff_cell_glow == null)
            {
                _eff_cell_glow = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_cell_glow.prefab", null, false);
            }
            return _eff_cell_glow;
        }
    }

    public GameObject eff_beijing_rain
    {
        get
        {
            if (_eff_beijing_rain == null)
            {
                _eff_beijing_rain = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_beijing_rain.prefab", null, false);
            }
            return _eff_beijing_rain;
        }

    }
    public GameObject eff_additional_first
    {
        get
        {
            if (_eff_additional_first == null)
            {
                _eff_additional_first = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_additional_first.prefab", null, false);
            }
            return _eff_additional_first;
        }

    }
    public GameObject eff_additional_second
    {
        get
        {
            if (_eff_additional_second == null)
            {
                _eff_additional_second = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_additional_second.prefab", null, false);
            }
            return _eff_additional_second;
        }

    }
    
    public GameObject eff_additional_box
    {
        get
        {
            if (_eff_additional_box == null)
            {
                _eff_additional_box = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_additional_box.prefab", null, false);
            }
            return _eff_additional_box;
        }

    }
    
    public GameObject eff_GOOD_glow
    {
        get
        {
            if (_eff_GOOD_glow == null)
            {
                _eff_GOOD_glow = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs/eff_GOOD_glow.prefab", null, false);
            }
            return _eff_GOOD_glow;
        }

    }

    public GameObject eff_Video
    {
        get
        {
            if (_eff_Video == null)
            {
                _eff_Video = null; // Framework.Asset.AssetsManager.Load<GameObject>("Assets/Effect/Prefabs2/eff_video.prefab", null, false);
            }
            return _eff_Video;
        }
    }
}
