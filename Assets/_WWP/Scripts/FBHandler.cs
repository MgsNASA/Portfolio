using Facebook.Unity;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WWP
{
    public class FBHandler
    {
        public IAppLinkResult result;

        public void Initialize()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(InitCallback, OnHideUnity);
            }
        }

        public void GetAppLinkRequest()
        {
            FB.GetAppLink(DeepLinkCallback);
        }

        private void DeepLinkCallback(IAppLinkResult _result)
        {
            if (_result != null && !string.IsNullOrEmpty(_result.Url))
            {
                var index = (new Uri(_result.Url)).Query.IndexOf("deeplink");
                if (index != -1)
                {
                    result = _result;
                }
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("failed to init Facebook");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        private static string ParseDeepLink(string link)
        {
            MatchCollection matches = Regex.Matches(link, Constants.FB_DEEPLINK_PATTERN);
            string[] subs = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                subs[i] = matches[i].Value;
            }
            if (subs.Length > 3)
            {
                string[] thirdSubParts = subs[2].Split("=");
                for (int i = 3; i < subs.Length; i++)
                {
                    string[] subPartsOverThree = subs[i].Split("=");
                    thirdSubParts[1] += $"_{subPartsOverThree[1]}";
                }
                subs[2] = thirdSubParts[0] + "=" + thirdSubParts[1];
            }
            string subPart = "";
            for (int i = 0; i < 3; i++)
            {
                string sub;
                if (i < subs.Length)
                {
                    sub = subs[i];
                }
                else
                {
                    sub = $"sub_id_{i + 1}=";
                }
                subPart += sub + "&";
            }
            subPart = subPart.Substring(0, subPart.Length - 1);
            return subPart;
        }
    }
}
