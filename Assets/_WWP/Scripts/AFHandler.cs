using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AppsFlyerSDK;
using Newtonsoft.Json;
using System.Text;

namespace WWP
{
    public class AFHandler
    {
        public static async UniTask GetConverstionData(AppsFlyerObjectScript afObj, Action<Dictionary<string, string>, string> callback)
        {
            bool afConverted = afObj.conversionData != null;
            if (!afConverted)
            {
                for (int i = 0; i < 5; i++)
                {
                    await UniTask.Delay(3000, true);
                    if (afObj.conversionData != null)
                    {
                        callback?.Invoke(ConvertToString(afObj.conversionData), ToBASE64(afObj.conversionDataString));
                        break;
                    }
                }
            }
            else
            {
                await UniTask.Delay(3000, true);
                callback?.Invoke(ConvertToString(afObj.conversionData), ToBASE64(afObj.conversionDataString));
            }
        }
        private static Dictionary<string, string> ConvertToString(Dictionary<string, object> conversionData)
        {
            //Print(conversionData);
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "campaign", GetValue(conversionData, "campaign") },
                { "campaign_id", GetValue(conversionData, "campaign_id") },

                { "adgroup", GetValue(conversionData, "adgroup") },
                { "adgroup_id", GetValue(conversionData, "adgroup_id") },

                { "adset", GetValue(conversionData, "adset") },
                { "adset_id", GetValue(conversionData, "adset_id") },

                { "af_siteid", GetValue(conversionData, "af_siteid") },
                { "traffic_source", GetValue(conversionData, "traffic_source") },
                { "appsflyer_id", GetValue(conversionData, "appsflyer_id") }
            };
            if (string.IsNullOrEmpty(data["appsflyer_id"]))
                data["appsflyer_id"] = AppsFlyer.getAppsFlyerId();
            return data;
        }

        private static string GetValue(Dictionary<string, object> conversionData, string key)
        {
            if (!conversionData.ContainsKey(key)) return "";
            if (conversionData[key] == null || conversionData[key].ToString() == "null") return "";
            return conversionData[key].ToString();
        }

        private static string ToBASE64(string conversionData)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(conversionData);
            string base64String = Convert.ToBase64String(jsonBytes);
            Debug.Log($"[AF HANDLER] conversion data base 64: {base64String}");
            return base64String;
        }

        private static void Print(Dictionary<string, object> dict)
        {
            var lines = dict.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
            string text = string.Join(Environment.NewLine, lines);
            Debug.Log("Raw conversion data is \n" + text);
        }
    }
}
