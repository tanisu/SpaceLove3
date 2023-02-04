using GoogleMobileAds.Api;
using UnityEngine;
using System;

public class ADManager : MonoBehaviour
{
    bool rewardFlag = false;
    RewardedAd rewarded;
    InterstitialAd inter;
    BannerView bannerView;
    string adUnitIdReward, adUnitIdInter, adUnitIdBanner;

    public static ADManager I;
    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
#if UNITY_ANDROID

        adUnitIdReward = "ca-app-pub-1205963622209231/1367589816";
        adUnitIdInter = "ca-app-pub-1205963622209231/2888539132";
        adUnitIdBanner = "ca-app-pub-1205963622209231/3993753150";
#elif UNITY_IPHONE
        adUnitIdReward = "ca-app-pub-1205963622209231/6595865764";
        adUnitIdInter =  "ca-app-pub-1205963622209231/7507839793";
        adUnitIdBanner = "ca-app-pub-1205963622209231/8820921462";
#else
#endif
        CreateAndLoadRewardedAd();
        RequestInterstitial();
        RequestBanner();
    }



    public void ShowAdmobReward()
    {
        if (rewarded.IsLoaded())
        {
            rewarded.Show();
        }
    }

    public void ShowInter()
    {
        if (inter.IsLoaded())
        {
            inter.Show();

        }
    }



    void Update()
    {
        if (rewardFlag)
        {
            rewardFlag = false;
            GameManager.I.GetItemFromAd();
        }
    }

    public void CreateAndLoadRewardedAd()
    {

        rewarded = new RewardedAd(adUnitIdReward);
        rewarded.OnAdLoaded += HandleRewardedAdLoaded;
        rewarded.OnAdFailedToLoad += HandleRewardedAdFailedToLoaded;
        rewarded.OnAdClosed += HandleRewardedAdClosed;
        rewarded.OnUserEarnedReward += HandleUserEarnedReward;
        //rewarded.OnAdClicked += HandleOnAdClicked;

        AdRequest request = new AdRequest.Builder().Build();

        rewarded.LoadAd(request);

    }

    public void HandleOnAdClicked()
    {

    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("reward ad loaded");
    }
    public void HandleRewardedAdFailedToLoaded(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log($"reward ad failed : {args.LoadAdError}");
    }
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        CreateAndLoadRewardedAd();
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        rewardFlag = true;
    }


    private void RequestInterstitial()
    {
        inter = new InterstitialAd(adUnitIdInter);
        inter.OnAdLoaded += HandleOnAdLoaded;
        inter.OnAdFailedToLoad += HandleOnAdFailedToLoaded;
        inter.OnAdClosed += HandleOnAdClosed;
        //inter.OnAdClicked += HandleOnAdClicked;
        AdRequest request = new AdRequest.Builder().Build();
        inter.LoadAd(request);
    }
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("inter loaded");
    }
    public void HandleOnAdFailedToLoaded(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log($"inter failed : {args.LoadAdError}");
    }
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        inter.Destroy();
        RequestInterstitial();
    }

    private void RequestBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        Debug.Log($"adaptiveSize {adaptiveSize}");
        bannerView = new BannerView(adUnitIdBanner, adaptiveSize, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);

    }
}
