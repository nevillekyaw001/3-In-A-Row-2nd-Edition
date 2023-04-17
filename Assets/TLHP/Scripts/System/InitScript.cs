﻿using UnityEngine;
using System.Collections;
using System;

using System.Collections.Generic;
using JuiceFresh.Scripts.System;
#if UNITY_ADS
using JuiceFresh.Scripts.Integrations;
using UnityEngine.Advertisements;
#endif

#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
#if  GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Target
{
    SCORE,
    COLLECT,
    ITEMS,
    BLOCKS,
    CAGES,
    BOMBS,
}

public enum LIMIT
{
    MOVES,
    TIME
}

public enum Ingredients
{
    None = 0,
    Ingredient1,
    Ingredient2,
    Ingredient3,
    Ingredient4

}

public enum CollectItems
{
    None = 0,
    Item1,
    Item2,
    Item3,
    Item4,
    Item5,
    Item6
}

public enum CollectStars
{
    STAR_1 = 1,
    STARS_2 = 2,
    STARS_3 = 3
}


public enum RewardedAdsType
{
    GetLifes,
    GetGems,
    GetGoOn
}

public class InitScript : MonoBehaviour
{
    public static InitScript Instance;
    public static int openLevel;


    public static float RestLifeTimer;
    public static string DateOfExit;
    public static DateTime today;
    public static DateTime DateOfRestLife;
    public static string timeForReps;
    private static int Lifes;

    public List<CollectedIngredients> collectedIngredients = new List<CollectedIngredients>();

    public RewardedAdsType currentReward;

    public static int lifes
    {
        get
        {
            return InitScript.Lifes;
        }
        set
        {
            InitScript.Lifes = value;
        }
    }

    public int CapOfLife = 5;
    public float TotalTimeForRestLifeHours = 0;
    public float TotalTimeForRestLifeMin = 15;
    public float TotalTimeForRestLifeSec = 60;
    public int FirstGems = 20;
    public static int Gems;
    public static int waitedPurchaseGems;
    private int BoostExtraMoves;
    private int BoostPackages;
    private int BoostStripes;
    private int BoostExtraTime;
    private int BoostBomb;
    private int BoostColorful_bomb;
    private int BoostHand;
    private int BoostRandom_color;
    public List<AdEvents> adsEvents = new List<AdEvents>();

    public static bool sound = false;
    public static bool music = false;
    private bool adsReady;
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public bool enableChartboostAds;
    public string rewardedVideoZone;
    public string nonRewardedVideoZone;
    public int ShowChartboostAdsEveryLevel;
    public int ShowAdmobAdsEveryLevel;
    private bool leftControl;
#if GOOGLE_MOBILE_ADS
	private InterstitialAd interstitial;
	private AdRequest requestAdmob;
#endif
    public string admobUIDAndroid;
    public string admobUIDIOS;

    public int ShowRateEvery;
    public string RateURL;
    public string RateURLIOS;
    private GameObject rate;
    public int rewardedGems = 5;
    public bool losingLifeEveryGame;
    public static Sprite profilePic;
    public GameObject facebookButton;
    //1.3.3
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;

    // Use this for initialization
    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
        //		if (Application.isEditor)//TODO comment it
        //			PlayerPrefs.DeleteAll ();

        DateOfExit = PlayerPrefs.GetString("DateOfExit", "");
        Gems = PlayerPrefs.GetInt("Gems");
        lifes = PlayerPrefs.GetInt("Lifes");
        if (PlayerPrefs.GetInt("Lauched") == 0)
        {    //First lauching
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            Gems = FirstGems;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.SetInt("Sound", 1);

            PlayerPrefs.SetInt("Lauched", 1);
            PlayerPrefs.Save();
        }
        rate = GameObject.Find("CanvasGlobal").transform.Find("Rate").gameObject;
        rate.SetActive(false);
        //rate.transform.SetParent(GameObject.Find("CanvasGlobal").transform);
        //rate.transform.localPosition = Vector3.zero;
        //rate.GetComponent<RectTransform>().anchoredPosition = (Resources.Load("Prefabs/Rate") as GameObject).GetComponent<RectTransform>().anchoredPosition;
        //rate.transform.localScale = Vector3.one;
        gameObject.AddComponent<InternetChecker>();
        GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
        SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");
#if UNITY_ADS//1.3
		enableUnityAds = true;
        var unityAds = Resources.Load<UnityAdsID>("UnityAdsID");
        #if UNITY_ANDROID
            Advertisement.Initialize(unityAds.androidID,false);
        #elif UNITY_IOS
            Advertisement.Initialize(unityAds.iOSID,false);
        #endif
#else
        enableUnityAds = false;
#endif
#if CHARTBOOST_ADS//1.4.1
		enableChartboostAds = true;
#else
        enableChartboostAds = false;
#endif


#if FACEBOOK
		FacebookManager fbManager = gameObject.AddComponent<FacebookManager> ();//1.3.3
		fbManager.facebookButton = facebookButton;//1.3.3
#endif

#if GOOGLE_MOBILE_ADS
		enableGoogleMobileAds = true;//1.3
#if UNITY_ANDROID
        MobileAds.Initialize(initStatus => { });
        interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
       MobileAds.Initialize(initStatus => { });
        interstitial = new InterstitialAd(admobUIDIOS);
#else
        MobileAds.Initialize(initStatus => { });
		interstitial = new InterstitialAd (admobUIDAndroid);
#endif

		// Create an empty ad request.
		requestAdmob = new AdRequest.Builder ().Build ();
		// Load the interstitial with the request.
		interstitial.LoadAd (requestAdmob);
		interstitial.OnAdLoaded += HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
#else
        enableGoogleMobileAds = false; //1.3
#endif
        Transform canvas = GameObject.Find("CanvasGlobal").transform;
        foreach (Transform item in canvas)
        {
            item.gameObject.SetActive(false);
        }
    }
#if GOOGLE_MOBILE_ADS
	
	public void HandleInterstitialLoaded (object sender, EventArgs args) {
		print ("HandleInterstitialLoaded event received.");
	}

	public void HandleInterstitialFailedToLoad (object sender, AdFailedToLoadEventArgs args) {
		print ("HandleInterstitialFailedToLoad event received with message: " + args.LoadAdError);
	}
#endif
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            leftControl = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            leftControl = false;

        if (CountDownTimer.instance.LoadClicked)//Input.GetKeyUp(KeyCode.U)
        {
            for (int i = 0; i < PlayerPrefs.GetInt("OpenLevel")+1; i++)
            {
                SaveLevelStarsCount(i, 1);
            }

        }
        
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);

    }

    private string GetLevelKey(int number)
    {
        return string.Format("Level.{0:000}.StarsCount", number);
    }

    public bool GetRewardedUnityAdsReady()
    {
#if UNITY_ADS

		rewardedVideoZone = "rewardedVideo";
		if (Advertisement.IsReady (rewardedVideoZone)) {
			return true;
		} else {
			rewardedVideoZone = "rewardedVideoZone";
			if (Advertisement.IsReady (rewardedVideoZone)) {
				return true;
			}
		}
#endif

        return false;
    }

    public void ShowRewardedAds()
    {
#if UNITY_ADS
		Debug.Log ("show Unity Rewarded ads video in " + LevelManager.THIS.gameStatus);

		if (GetRewardedUnityAdsReady ()) {
			Advertisement.Show (rewardedVideoZone, new ShowOptions {
				resultCallback = result => {
					if (result == ShowResult.Finished) {
						CheckRewardedAds ();
					}
				}
			});
		}
#elif GOOGLE_MOBILE_ADS//2.2
        bool stillShow = true;
#if UNITY_ADS
        stillShow = !GetRewardedUnityAdsReady ();
#endif
        if(stillShow)
        {
            Debug.Log("show Admob Rewarded ads video in " + LevelManager.THIS.gameStatus);
            RewAdmobManager.THIS.ShowRewardedAd(CheckRewardedAds);
        }
#endif
    }

    public void CheckAdsEvents(GameState state)
    {

        foreach (AdEvents item in adsEvents)
        {
            if (item.gameEvent == state)
            {
                if ((LevelManager.THIS.gameStatus == GameState.GameOver || LevelManager.THIS.gameStatus == GameState.Pause ||
                    LevelManager.THIS.gameStatus == GameState.Playing || LevelManager.THIS.gameStatus == GameState.PrepareGame || LevelManager.THIS.gameStatus == GameState.PreWinAnimations ||
                    LevelManager.THIS.gameStatus == GameState.RegenLevel || LevelManager.THIS.gameStatus == GameState.Win))
                {
                    item.calls++;
                    if (item.calls % item.everyLevel == 0)
                        ShowAdByType(item.adType);
                    // } else {
                    // ShowAdByType (item.adType);

                }
            }
        }
    }

    void ShowAdByType(AdType adType)
    {
        if (adType == AdType.AdmobInterstitial)
            ShowAds(false);
        else if (adType == AdType.UnityAdsVideo)
            ShowVideo();
        else if (adType == AdType.ChartboostInterstitial)
            ShowAds(true);

    }

    public void ShowVideo()
    {
        Debug.Log("show Unity ads video on " + LevelManager.THIS.gameStatus);
#if UNITY_ADS

		if (Advertisement.IsReady ("video")) {
			Advertisement.Show ("video");
		} else {
			if (Advertisement.IsReady ("defaultZone")) {
				Advertisement.Show ("defaultZone");
			}
		}
#endif
    }

    public void ShowAds(bool chartboost = true)
    {
        if (chartboost)
        {
            Debug.Log("show Chartboost Interstitial on " + LevelManager.THIS.gameStatus);
#if CHARTBOOST_ADS
			Chartboost.showInterstitial (CBLocation.Default);
			Chartboost.cacheInterstitial (CBLocation.Default);
#endif
        }
        else
        {
            Debug.Log("show Google mobile ads Interstitial on " + LevelManager.THIS.gameStatus);
#if GOOGLE_MOBILE_ADS
			if (interstitial.IsLoaded ()) {
				interstitial.Show ();
#if UNITY_ANDROID
				interstitial = new InterstitialAd (admobUIDAndroid);
#elif UNITY_IOS
                interstitial = new InterstitialAd(admobUIDIOS);
#else
				interstitial = new InterstitialAd (admobUIDAndroid);
#endif

				// Create an empty ad request.
				requestAdmob = new AdRequest.Builder ().Build ();
				// Load the interstitial with the request.
				interstitial.LoadAd (requestAdmob);
			}
#endif
        }
    }

    public void ShowRate()
    {
        InternetChecker.THIS.CheckInternet(true, (isConnected) =>
        {
            if (isConnected) rate.SetActive(true);
        });
    }


    public void CheckRewardedAds()
    {
        RewardIcon reward = GameObject.Find("CanvasGlobal").transform.Find("Reward").GetComponent<RewardIcon>();
        if (currentReward == RewardedAdsType.GetGems)
        {
            reward.SetIconSprite(0);

            reward.gameObject.SetActive(true);
            AddGems(rewardedGems);
            GameObject.Find("CanvasGlobal").transform.Find("GemsShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetLifes)
        {
            reward.SetIconSprite(1);
            reward.gameObject.SetActive(true);
            RestoreLifes();
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").GetComponent<AnimationManager>().CloseMenu();
        }
        else if (currentReward == RewardedAdsType.GetGoOn)
        {
            GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").GetComponent<AnimationManager>().GoOnFailed();
        }

    }

    public void SetGems(int count)
    {//1.3.3
        Gems = count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }

    public void AddGems(int count)
    {
        Gems += count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
		NetworkManager.currencyManager.IncBalance (count);
#endif
    }

    public void SpendGems(int count)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
        Gems -= count;
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
		NetworkManager.currencyManager.DecBalance (count);
#endif
    }


    public void RestoreLifes()
    {
        lifes = CapOfLife;
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.Save();
    }

    public void AddLife(int count)
    {
        lifes += count;
        if (lifes > CapOfLife)
            lifes = CapOfLife;
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.Save();
    }

    public int GetLife()
    {
        if (lifes > CapOfLife)
        {
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }
        return lifes;
    }

    public void PurchaseSucceded()
    {
        AddGems(waitedPurchaseGems);
        waitedPurchaseGems = 0;
    }

    public void SpendLife(int count)
    {
        if (lifes > 0)
        {
            lifes -= count;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }
        //else
        //{
        //    GameObject.Find("Canvas").transform.Find("RestoreLifes").gameObject.SetActive(true);
        //}
    }

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        PlayerPrefs.SetInt("" + boostType, count);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
		NetworkManager.dataManager.SetBoosterData ();
#endif

        //   ReloadBoosts();
    }

    public void SpendBoost(BoostType boostType)
    {
        PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) - 1);
        PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
		NetworkManager.dataManager.SetBoosterData ();
#endif

    }
    //void ReloadBoosts()
    //{
    //    BoostExtraMoves = PlayerPrefs.GetInt("" + BoostType.ExtraMoves);
    //    BoostPackages = PlayerPrefs.GetInt("" + BoostType.Packages);
    //    BoostStripes = PlayerPrefs.GetInt("" + BoostType.Stripes);
    //    BoostExtraTime = PlayerPrefs.GetInt("" + BoostType.ExtraTime);
    //    BoostBomb = PlayerPrefs.GetInt("" + BoostType.Bomb);
    //    BoostColorful_bomb = PlayerPrefs.GetInt("" + BoostType.Colorful_bomb);
    //    BoostHand = PlayerPrefs.GetInt("" + BoostType.Hand);
    //    BoostRandom_color = PlayerPrefs.GetInt("" + BoostType.Random_color);

    //}
    //public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    //{
    //    PurchaseSucceded();
    //}

    void OnApplicationFocus(bool focusStatus)
    {//1.3.3
        if (MusicBase.Instance)
        {
            MusicBase.Instance.GetComponent<AudioSource>().Play();
        }
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (RestLifeTimer > 0)
            {
                PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            }
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {   //1.4  added 
        if (RestLifeTimer > 0)
        {
            PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
        }
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    public void OnLevelClicked(object sender, LevelReachedEventArgs args)
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;
        if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf)
        {
            PlayerPrefs.SetInt("OpenLevel", args.Number);
            PlayerPrefs.Save();
            LevelManager.THIS.MenuPlayEvent();
            LevelManager.THIS.LoadLevel();
            openLevel = args.Number;
            //  currentTarget = targets[args.Number];
            GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
        }
    }

    void OnEnable()
    {
        LevelsMap.LevelSelected += OnLevelClicked;
    }

    void OnDisable()
    {
        LevelsMap.LevelSelected -= OnLevelClicked;

        //		if(RestLifeTimer>0){
        PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
        //		}
        PlayerPrefs.SetInt("Lifes", lifes);
        PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
        PlayerPrefs.Save();
#if GOOGLE_MOBILE_ADS
		interstitial.OnAdLoaded -= HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
#endif

    }


}
