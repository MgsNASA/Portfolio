using Cysharp.Threading.Tasks;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace WWP
{
    public class FBHandler
    {
        public static async UniTask GetLink(Action<string> callback)
        {
            string link = null;
            GetLink_Internal((l) => link = l).Forget();
            for (int i = 0; i < 2; i++)
            {
                if (link != null)
                {
                    callback?.Invoke(link);
                    break;
                }
                await UniTask.Delay(3000, true);
                if (link != null)
                {
                    callback?.Invoke(link);
                    break;
                }
            }
        }

        private static async UniTask GetLink_Internal(Action<string> callback)
        {
            string url = $"https://graph.facebook.com/{Constants.FB_APP_ID}?fields=app_links&access_token={Constants.FB_CLIENT_TOKEN}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 3;
                try
                {
                    var req = await request.SendWebRequest();
                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        string response = req.downloadHandler.text;
                        byte[] bytes = Encoding.UTF8.GetBytes(response);
                        string base64String = Convert.ToBase64String(bytes);
                        callback?.Invoke(base64String);
                    }
                    else
                    {
                        Debug.Log("Failed to get deep link: " + req.error);
                        callback?.Invoke("");
                    }
                }
                catch (UnityWebRequestException)
                {

                }
            }
            //callback?.Invoke(ParseDeepLink("app://?sub_id_1=1111&sub_id_2=2222&sub_id_3=3333&sub_id_5=5533&sub_id_10=1033"));
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
