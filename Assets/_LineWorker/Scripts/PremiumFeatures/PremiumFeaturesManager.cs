using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gley.MobileAds;


namespace _LineWorker
{
    public class PremiumFeaturesManager : MonoBehaviour
    {
        public static PremiumFeaturesManager Instance { get; private set; }

        public bool enablePremiumFeatures = true;

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


            if (enablePremiumFeatures)
            {
                API.Initialize();
            }

        }
    }
}