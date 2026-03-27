using System;

namespace TaskForce.AP.Client.Core
{
    public interface IAdvertisementPlayer
    {
        event EventHandler InterstitialAdvertisementClosedEvent;
        bool CanPlayInterstitialAdvertisement();
        void PlayInterstitialAdvertisement();
        void EnablePersonalizedAds(bool isPersonalizedAdsAllowed);
    }
}
