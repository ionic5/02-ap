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
        private string _adUnitID;
        [SerializeField]
        private float _retryDelay;
        [SerializeField]
        private GameObject _adsBackground;

        private InterstitialAd _interstitialAd;
        private bool _isPersonalizedAdsAllowed;
        private bool _isLoading;
        private bool _isLoadQueued;
        private bool _isInitialized;

        public Core.ILogger Logger;
        public Core.Timer RetryTimer;
        public SynchronizationContext SynchronizationContext;

        public event EventHandler InterstitialAdvertisementClosedEvent;

        public void Initialize()
        {
            _isPersonalizedAdsAllowed = true;
            _isLoading = false;
            _isLoadQueued = false;
            _isInitialized = false;

            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
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
        }

        private void LoadInterstitialAd()
        {
            if (_isLoading || !_isInitialized)
            {
                _isLoadQueued = true;
                return;
            }
            _isLoading = true;

            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

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

            InterstitialAd.Load(_adUnitID, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    SynchronizationContext?.Post(_ =>
                    {
                        OnLoaded(ad, error);
                    }, null);
                });
        }

        private void OnLoaded(InterstitialAd ad, LoadAdError error)
        {
            if (error != null || ad == null)
            {
                Logger.Warn("interstitial ad failed to load an ad " +
                                "with error : " + error);
                _isLoading = false;

                SetDelayedLoadInterstitialAd();
                return;
            }

            Logger.Info("Interstitial ad loaded with response : "
                + ad.GetResponseInfo());

            _interstitialAd = ad;

            RegisterEventHandlers(_interstitialAd);

            _isLoading = false;
            if (_isLoadQueued)
            {
                _isLoadQueued = false;
                LoadInterstitialAd();
            }
        }

        private void SetDelayedLoadInterstitialAd()
        {
            if (RetryTimer.IsRunning())
                RetryTimer.Stop();

            RetryTimer.Start(_retryDelay, () => { LoadInterstitialAd(); });
        }

        private void RegisterEventHandlers(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    OnAdFullScreenContentClosed();
                }, null);
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                SynchronizationContext?.Post(_ =>
                {
                    OnAdFullScreenContentFailed(error);
                }, null);
            };
        }

        private void OnAdFullScreenContentFailed(AdError error)
        {
            Logger.Warn("Interstitial ad failed to open full screen content " +
                            "with error : " + error);

            SetDelayedLoadInterstitialAd();

            _adsBackground.SetActive(false);
        }

        private void OnAdFullScreenContentClosed()
        {
            Logger.Info("close Scene. Interstitial Advertisement Closed Event fired.");

            InterstitialAdvertisementClosedEvent?.Invoke(this, EventArgs.Empty);
            InterstitialAdvertisementClosedEvent = null;

            LoadInterstitialAd();

            _adsBackground.SetActive(false);
        }

        public bool CanPlayInterstitialAdvertisement()
        {
            return _interstitialAd != null && _interstitialAd.CanShowAd();
        }

        public void PlayInterstitialAdvertisement()
        {
            if (!CanPlayInterstitialAdvertisement())
            {
                Logger.Warn("Interstitial ad is not ready yet.");
                return;
            }

            _interstitialAd.Show();

            _adsBackground.SetActive(true);
        }

        public void EnablePersonalizedAds(bool isPersonalizedAdsAllowed)
        {
            _isPersonalizedAdsAllowed = isPersonalizedAdsAllowed;
            Logger.Info($"NPA setting changed to: {isPersonalizedAdsAllowed}. Reloading ad.");

            LoadInterstitialAd();
        }
    }
}