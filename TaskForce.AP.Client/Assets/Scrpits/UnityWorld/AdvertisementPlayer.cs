using GoogleMobileAds.Api;
using System;
using System.Threading;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class AdvertisementPlayer : MonoBehaviour, IAdvertisementPlayer
    {
        [SerializeField]
        private string _interstitialAdUnitID;
        [SerializeField]
        private string _rewardedAdUnitID;
        [SerializeField]
        private float _retryDelay;
        [SerializeField]
        private GameObject _adsBackground;

        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;

        private bool _isPersonalizedAdsAllowed;

        private bool _isInterstitialLoading;
        private bool _isInterstitialLoadQueued;

        private bool _isRewardedLoading;
        private bool _isRewardedLoadQueued;

        private bool _isInitialized;

        private Action _interstitialOnClosed;
        private Action _rewardedOnRewarded;
        private Action _rewardedOnClosed;

        public Core.ILogger Logger;
        public Core.Timer RetryTimer;
        public Core.Timer RewardedRetryTimer;
        public SynchronizationContext SynchronizationContext;

        public void Initialize()
        {
            _isPersonalizedAdsAllowed = true;
            _isInterstitialLoading = false;
            _isInterstitialLoadQueued = false;
            _isRewardedLoading = false;
            _isRewardedLoadQueued = false;
            _isInitialized = false;

            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // Google Mobile Ads SDK의 콜백은 백그라운드 스레드에서 발생한다.
                // Unity의 MonoBehaviour, GameObject 조작은 메인 스레드에서만 허용되므로
                // SynchronizationContext.Post()를 통해 콜백을 메인 스레드로 전환한다.
                SynchronizationContext?.Post(_ =>
                {
                    OnInitialized();
                }, null);
            });
        }

        private void OnInitialized()
        {
            _isInitialized = true;

            LoadInterstitialAd();
            LoadRewardedAd();
        }

        private void LoadInterstitialAd()
        {
            if (_isInterstitialLoading || !_isInitialized)
            {
                _isInterstitialLoadQueued = true;
                return;
            }
            _isInterstitialLoading = true;

            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            var adRequest = BuildAdRequest();

            InterstitialAd.Load(_interstitialAdUnitID, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    SynchronizationContext?.Post(_ =>
                    {
                        OnInterstitialLoaded(ad, error);
                    }, null);
                });
        }

        private void OnInterstitialLoaded(InterstitialAd ad, LoadAdError error)
        {
            if (error != null || ad == null)
            {
                Logger.Warn("Interstitial ad failed to load with error : " + error);
                _isInterstitialLoading = false;
                SetDelayedLoadInterstitialAd();
                return;
            }

            Logger.Info("Interstitial ad loaded with response : " + ad.GetResponseInfo());

            _interstitialAd = ad;
            RegisterInterstitialEventHandlers(_interstitialAd);

            _isInterstitialLoading = false;
            if (_isInterstitialLoadQueued)
            {
                _isInterstitialLoadQueued = false;
                LoadInterstitialAd();
            }
        }

        private void SetDelayedLoadInterstitialAd()
        {
            if (RetryTimer.IsRunning())
                RetryTimer.Stop();

            RetryTimer.Start(_retryDelay, () => { LoadInterstitialAd(); });
        }

        private void RegisterInterstitialEventHandlers(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    OnInterstitialFullScreenContentClosed();
                }, null);
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    OnInterstitialFullScreenContentFailed(error);
                }, null);
            };
        }

        private void OnInterstitialFullScreenContentFailed(AdError error)
        {
            Logger.Warn("Interstitial ad failed to open full screen content with error : " + error);

            SetDelayedLoadInterstitialAd();
            _adsBackground.SetActive(false);

            var callback = _interstitialOnClosed;
            _interstitialOnClosed = null;
            callback?.Invoke();
        }

        private void OnInterstitialFullScreenContentClosed()
        {
            Logger.Info("Interstitial ad closed.");

            LoadInterstitialAd();
            _adsBackground.SetActive(false);

            var callback = _interstitialOnClosed;
            _interstitialOnClosed = null;
            callback?.Invoke();
        }

        public bool CanPlayInterstitialAdvertisement()
        {
            return _interstitialAd != null && _interstitialAd.CanShowAd();
        }

        public void PlayInterstitialAdvertisement(Action onClosed)
        {
            if (!CanPlayInterstitialAdvertisement())
            {
                Logger.Warn("Interstitial ad is not ready yet.");
                return;
            }

            _interstitialOnClosed = onClosed;
            _interstitialAd.Show();
            _adsBackground.SetActive(true);
        }

        private void LoadRewardedAd()
        {
            if (_isRewardedLoading || !_isInitialized)
            {
                _isRewardedLoadQueued = true;
                return;
            }
            _isRewardedLoading = true;

            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = BuildAdRequest();

            RewardedAd.Load(_rewardedAdUnitID, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    SynchronizationContext?.Post(_ =>
                    {
                        OnRewardedLoaded(ad, error);
                    }, null);
                });
        }

        private void OnRewardedLoaded(RewardedAd ad, LoadAdError error)
        {
            if (error != null || ad == null)
            {
                Logger.Warn("Rewarded ad failed to load with error : " + error);
                _isRewardedLoading = false;
                SetDelayedLoadRewardedAd();
                return;
            }

            Logger.Info("Rewarded ad loaded with response : " + ad.GetResponseInfo());

            _rewardedAd = ad;
            RegisterRewardedEventHandlers(_rewardedAd);

            _isRewardedLoading = false;
            if (_isRewardedLoadQueued)
            {
                _isRewardedLoadQueued = false;
                LoadRewardedAd();
            }
        }

        private void SetDelayedLoadRewardedAd()
        {
            if (RewardedRetryTimer.IsRunning())
                RewardedRetryTimer.Stop();

            RewardedRetryTimer.Start(_retryDelay, () => { LoadRewardedAd(); });
        }

        private void RegisterRewardedEventHandlers(RewardedAd rewardedAd)
        {
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    OnRewardedFullScreenContentClosed();
                }, null);
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    OnRewardedFullScreenContentFailed(error);
                }, null);
            };
        }

        private void OnRewardedFullScreenContentFailed(AdError error)
        {
            Logger.Warn("Rewarded ad failed to open full screen content with error : " + error);

            SetDelayedLoadRewardedAd();
            _adsBackground.SetActive(false);

            var callback = _rewardedOnClosed;
            _rewardedOnRewarded = null;
            _rewardedOnClosed = null;
            callback?.Invoke();
        }

        private void OnRewardedFullScreenContentClosed()
        {
            Logger.Info("Rewarded ad closed.");

            LoadRewardedAd();
            _adsBackground.SetActive(false);

            var callback = _rewardedOnClosed;
            _rewardedOnRewarded = null;
            _rewardedOnClosed = null;
            callback?.Invoke();
        }

        public bool CanPlayRewardedAdvertisement()
        {
            return _rewardedAd != null && _rewardedAd.CanShowAd();
        }

        public void PlayRewardedAdvertisement(Action onRewarded, Action onClosed)
        {
            if (!CanPlayRewardedAdvertisement())
            {
                Logger.Warn("Rewarded ad is not ready yet.");
                return;
            }

            _rewardedOnRewarded = onRewarded;
            _rewardedOnClosed = onClosed;

            _rewardedAd.Show((Reward reward) =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    var callback = _rewardedOnRewarded;
                    _rewardedOnRewarded = null;
                    callback?.Invoke();
                }, null);
            });

            _adsBackground.SetActive(true);
        }

        private AdRequest BuildAdRequest()
        {
            var adRequest = new AdRequest();

            if (_isPersonalizedAdsAllowed)
            {
                Logger.Info("Ad Request: Personalized Ads will be requested (NPA not set).");
            }
            else
            {
                adRequest.Extras.Add("npa", "1");
                Logger.Info("Ad Request: NPA is set to 1 (Non-Personalized Ads).");
            }

            return adRequest;
        }

        public void EnablePersonalizedAds(bool isPersonalizedAdsAllowed)
        {
            _isPersonalizedAdsAllowed = isPersonalizedAdsAllowed;
            Logger.Info($"NPA setting changed to: {isPersonalizedAdsAllowed}. Reloading ads.");

            LoadInterstitialAd();
            LoadRewardedAd();
        }
    }
}
