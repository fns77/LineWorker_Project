using UnityEngine;
using System.Collections;
using Gley.MobileAds;

namespace _LineWorker
{
    public class AdDisplayer : MonoBehaviour
    {
        

        public enum BannerAdPos
        {
            Top,
            Bottom
        }
        public static AdDisplayer Instance { get; private set; }

        [Header("Banner Ad Display Config")]
        [Tooltip("Whether or not to show banner ad")]
        public bool showBannerAd = true;
        public BannerAdPos bannerAdPosition = BannerAdPos.Bottom;

        [Header("Interstitial Ad Display Config")]
        [Tooltip("Whether or not to show interstitial ad")]
        public bool showInterstitialAd = true;
        [Tooltip("Show interstitial ad every [how many] games")]
        public int gamesPerInterstitial = 3;
        [Tooltip("How many seconds after game over that interstitial ad is shown")]
        public float showInterstitialDelay = 2f;

        [Header("Rewarded Ad Display Config")]
        [Tooltip("Check to allow watching ad to earn coins")]
        public bool watchAdToEarnCoins = true;
        [Tooltip("How many coins the user earns after watching a rewarded ad")]
        public int rewardedCoins = 50;

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        //public static event System.Action CompleteRewardedAdToRecoverLostGame;
        public static event System.Action CompleteRewardedAdToEarnCoins;

        private static int gameCount = 0;
        
        void OnEnable()
        {
            GameManager.GameStateChanged += OnGameStateChanged;
        }

        void OnDisable()
        {
            GameManager.GameStateChanged -= OnGameStateChanged;
        }

        void Start()
        {
            // Show banner ad
            if (showBannerAd)
            {
                //API.ShowBanner(BannerPosition.Bottom,BannerType.Adaptive);
                //API.ShowBanner(bannerAdPosition == BannerAdPos.Top ? BannerPosition.Top : BannerPosition.Bottom, BannerType.Adaptive);
                Invoke("ShowBanner", 1f);
            }
        }

        public void ShowBanner()
        {
            API.ShowBanner(bannerAdPosition == BannerAdPos.Top ? BannerPosition.Top : BannerPosition.Bottom, BannerType.Adaptive);
        }

        void OnGameStateChanged(GameState newState, GameState oldState)
        {       
            if (newState == GameState.GameOver)
            {
                // Show interstitial ad
                if (showInterstitialAd)
                {
                    gameCount++;

                    if (gameCount >= gamesPerInterstitial)
                    {
                        if (API.IsInterstitialAvailable())
                        {
                            // Show default ad after some optional delay
                            StartCoroutine(ShowInterstitial(showInterstitialDelay));

                            // Reset game count
                            gameCount = 0;
                        }
                    }
                }
            }
        }

        IEnumerator ShowInterstitial(float delay = 0f)
        {        
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            API.ShowInterstitial();
        }

        public bool CanShowRewardedAd()
        {
            return API.IsRewardedVideoAvailable();
        }

        //public void ShowRewardedAdToRecoverLostGame()
        //{
        //    if (CanShowRewardedAd())
        //    {
        //        Advertising.RewardedAdCompleted += OnCompleteRewardedAdToRecoverLostGame;
        //        Advertising.ShowRewardedAd();
        //    }
        //}

        //void OnCompleteRewardedAdToRecoverLostGame(RewardedAdNetwork adNetwork, AdLocation location)
        //{
        //    // Unsubscribe
        //    Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToRecoverLostGame;

        //    // Fire event
        //    if (CompleteRewardedAdToRecoverLostGame != null)
        //    {
        //        CompleteRewardedAdToRecoverLostGame();
        //    }
        //}

        public void ShowRewardedAdToEarnCoins()
        {
            if (CanShowRewardedAd())
            {
                API.ShowRewardedVideo(OnCompleteRewardedAdToEarnCoins);
                
            }
        }

        public void ShowIntersialAds()
        {
            if (API.IsInterstitialAvailable())
            {
                API.ShowInterstitial();
            }
        }

        void OnCompleteRewardedAdToEarnCoins(bool completed)
        {
            if (completed)
            {
                if(CompleteRewardedAdToEarnCoins != null)
                {
                    CompleteRewardedAdToEarnCoins();
                }
            }
        }
    }
}
