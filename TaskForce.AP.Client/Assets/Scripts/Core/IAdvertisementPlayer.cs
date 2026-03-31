using System;

namespace TaskForce.AP.Client.Core
{
    public interface IAdvertisementPlayer
    {
        bool CanPlayInterstitialAdvertisement();
        void PlayInterstitialAdvertisement(Action onClosed);
        bool CanPlayRewardedAdvertisement();
        void PlayRewardedAdvertisement(Action onRewarded, Action onClosed);
        void EnablePersonalizedAds(bool isPersonalizedAdsAllowed);
    }
}
