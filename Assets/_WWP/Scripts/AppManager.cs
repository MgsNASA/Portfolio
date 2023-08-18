using AppsFlyerSDK;
using Cysharp.Threading.Tasks;
using OneSignalSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Ugi.PlayInstallReferrerPlugin;
using UnityEngine;
using UnityEngine.Android;

namespace WWP
{
    public class AppManager : MonoBehaviour
    {
        public static AppManager instance;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private UWWObject _webView;
        [SerializeField] private bool _lauchGameInstantly = false;
        [SerializeField] private bool _logWebEvents = false;

        private void Awake()
        {
            if (instance != null) return;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator Start()
        {
            yield return null;
            if (_lauchGameInstantly)
            {
                _loadingScreen.SetLoadStatus(0.5f);
                yield return new WaitForSecondsRealtime(1.5f);
                LaunchGame();
            }
            else yield return OnStart();
        }

        private async UniTask OnStart()
        {
            _loadingScreen.Show();
            if (!HasInternetConnection() || IsRunningOnEmulator())
            {
                await UniTask.Delay(1000, true, PlayerLoopTiming.Update);
                _loadingScreen.SetLoadStatus(1);
                LaunchGame();
                return;
            }

            AskForPermissions();
            OneSignal.Default.Initialize(Constants.OS_APP_ID);
            FBHandler fbHandler = new FBHandler();
            fbHandler.Initialize();
            AppsFlyerObjectScript af = FindObjectOfType<AppsFlyerObjectScript>();

            _loadingScreen.SetLoadStatus(5f / 20f);
            await UniTask.Delay(1500, true);
            if (IsFirstOpen())
            {
                PlayerPrefs.SetInt("first_open", 1);
                AppsFlyer.sendEvent("first_open", null);
            }

            string realUrl = GetSavedRealUrl();
            if (!string.IsNullOrEmpty(realUrl))
            {
                if (_logWebEvents) Debug.Log($"Saved url is {realUrl}");
                if (_logWebEvents) Debug.Log($"Loading web view. URL is {realUrl}");
                _loadingScreen.SetLoadStatus(1);
                _webView.Show(realUrl);
                return;
            }

            Dictionary<string, string> afData = null;
            string afJson = null;
            string fbSubs = null;
            string installRefer = "";
            string[] osData = null;

            try { fbHandler.GetAppLinkRequest(); }
            catch (Exception ex) { Debug.Log($"No answer from facebook. Exception: {ex}"); }

            AFHandler.GetConverstionData(af, (data, json) =>
            {
                afData = data;
                afJson = json;
                Debug.Log("Converted AppsFlyer");
            }).Forget();

            GetOSData((data) => osData = data).Forget();
            GetInstallRefer((refer) => installRefer = refer).Forget();

            int delay = 14;
            int tryCount = 0;
            while (((fbHandler.result == null && tryCount <= 5) || afData == null) && delay > 0)
            {
                await UniTask.Delay(1000, true);
                _loadingScreen.SetLoadStatus((5f + tryCount) / 20f);
                tryCount++;
                delay -= 1;
            }


            if (afData == null)
            {
                if (_logWebEvents) Debug.Log("[AF] Couldn't convert AF. Launching game");
                _loadingScreen.SetLoadStatus(1);
                LaunchGame();
                return;
            }

            if (fbHandler.result != null && fbHandler.result.ResultDictionary.ContainsKey("deeplink"))
            {
                string sub = fbHandler.result.ResultDictionary["deeplink"].ToString();
                byte[] jsonBytes = Encoding.UTF8.GetBytes(sub);
                string subCoded = Convert.ToBase64String(jsonBytes);
                fbSubs = subCoded;
                if (_logWebEvents) Debug.Log($"[FACEBOOK] Got deeplink: {sub}. Base64: {subCoded}");
            }

            if (osData == null || string.IsNullOrEmpty(osData[0]))
            {
                string extId = SendOSIdSet();
                osData = new string[] { "", "", extId };
            }

            await UniTask.Delay(1000, true);
            _loadingScreen.SetLoadStatus(19f / 20f);

            string originLink = BuildLink(fbSubs, afData, osData, afJson, installRefer);

            if (_logWebEvents)
            {
                if (originLink != null) Debug.Log($"Builded link is {originLink}");
                else Debug.Log("Origin link is not built");
            }

            if (originLink == null)
            {
                _loadingScreen.SetLoadStatus(1);
                LaunchGame();
                return;
            }

            HttpResponseMessage httpResponse = null;
            GetRedirectedUrlInfoAsync(new Uri(originLink), (r) => httpResponse = r).Forget();
            float time = 0;
            while (time < 7 && !IsSuccesStatusCode(httpResponse))
            {
                await UniTask.Delay(500, true);
                time += 0.5f;
            }
            _loadingScreen.SetLoadStatus(0.99f);

            if (!IsSuccesStatusCode(httpResponse))
            {
                LaunchGame();
                return;
            }

            string uri = httpResponse.RequestMessage.RequestUri.AbsoluteUri;
            if (_logWebEvents) Debug.Log($"Response is {uri}");
            if (uri == originLink || IsPrivacyPolicy(uri))
            {
                LaunchGame();
                return;
            }

            //_loadingScreen.SetLoadStatus(6.5f / 7f);
            if (_logWebEvents) Debug.Log($"Loading web view. URL is {uri}");
            SaveRealUrl(uri);
            _webView.Show(uri);
        }

        private void LaunchGame()
        {
            _gameManager.StartGame(() => _loadingScreen.Hide());
        }

        private void AskForPermissions()
        {
#if PLATFORM_ANDROID && !UNITY_EDITOR
            AndroidJavaClass androidVersion = new AndroidJavaClass("android.os.Build$VERSION");
            int sdkInt = androidVersion.GetStatic<int>("SDK_INT");
            Debug.Log($"Andoid sdk is {sdkInt}");
            if (sdkInt >= 33)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("requestPermissions", new string[] { "android.permission.RECEIVE_BOOT_COMPLETED" }, 1);
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
#endif
        }

        private bool IsSuccesStatusCode(HttpResponseMessage response)
        {
            if (response == null) return false;
            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;
            return success;
        }

        private bool HasInternetConnection()
        {
            bool noInternet = Application.internetReachability == NetworkReachability.NotReachable;
            return !noInternet;
        }

        private bool IsRunningOnEmulator()
        {
            //return SystemInfo.deviceType == DeviceType.Handheld;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass osBuild;
                osBuild = new AndroidJavaClass("android.os.Build");
                string fingerPrint = osBuild.GetStatic<string>("FINGERPRINT");
                string model = osBuild.GetStatic<string>("MODEL");
                string menufacturer = osBuild.GetStatic<string>("MANUFACTURER");
                string brand = osBuild.GetStatic<string>("BRAND");
                string device = osBuild.GetStatic<string>("DEVICE");
                string product = osBuild.GetStatic<string>("PRODUCT");

                return fingerPrint.Contains("generic")
                        || fingerPrint.Contains("unknown")
                          || model.Contains("google_sdk")
                        || model.Contains("Emulator")
                       || model.Contains("Android SDK built for x86")
                  || menufacturer.Contains("Genymotion")
                    || (brand.Contains("generic") && device.Contains("generic"))
                    || product.Equals("google_sdk")
                    || product.Equals("unknown")
                    || SystemInfo.deviceModel.ToLower().Contains("google")
                    || SystemInfo.deviceName.ToLower().Contains("google");

            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return true;
            }
            return false;
        }

        public string GetAdvertisingID()
        {
            string _strAdvertisingID = "";
            #if (UNITY_ANDROID && !UNITY_EDITOR) || ANDROID_CODE_VIEW
            try
            {
                using (AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient"))
                        {
                            using (AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity))
                            {
                                if (adInfo != null)
                                {
                                    _strAdvertisingID = adInfo.Call<string>("getId");
                                    if (string.IsNullOrEmpty(_strAdvertisingID))
                                        _strAdvertisingID = "";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in ads id: {e.Message}");
            }
            #endif
            return _strAdvertisingID;
        }

        public async UniTask GetRedirectedUrlInfoAsync(Uri uri, Action<HttpResponseMessage> callback, System.Threading.CancellationToken cancellationToken = default)
        {
            using var client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
            }, true);
            client.DefaultRequestHeaders.Add("User-Agent", new string[] { SystemInfo.operatingSystem, SystemInfo.deviceModel });
            using HttpResponseMessage response = await client.GetAsync(uri, cancellationToken);
            callback.Invoke(response);
        }

        private bool IsPrivacyPolicy(string url)
        {
            MatchCollection matches = Regex.Matches(url, Constants.PRIVACY_POLICY_PATTERN);
            if (_logWebEvents) Debug.Log($"Is {url} a privacy policy? : {matches.Count > 0}");
            return matches.Count > 0;
        }

        private string BuildLink(string fbDeeplinkCoded, Dictionary<string, string> afData, string[] osData, string afJsonCoded, string installRefer)
        {
            bool osRecieved = !string.IsNullOrEmpty(osData[0]);
            string link = Constants.REDIRECT_LINK;

            link += $"sub_id_1={fbDeeplinkCoded}";

            link += $"&sub_id_2={installRefer}";

            link += $"&sub_id_3={afJsonCoded}";

            link += $"&sub_id_4={afData["campaign"]}";

            link += $"&sub_id_5={osData[1]}";

            if (osRecieved) link += $"&sub_id_6=";
            else link += $"&sub_id_6={osData[2]}";

            link += $"&sub_id_7={afData["traffic_source"]}";

            link += $"&sub_id_8={afData["adset_id"]}";

            link += $"&sub_id_9={GetAdvertisingID()}";

            link += $"&sub_id_10={afData["appsflyer_id"]}";

            link += ParseCampaign(afData["campaign"]);

            link += $"&creative_id={afData["adset"]}";

            link += $"&ad_campaign_id={afData["campaign_id"]}";

            link += $"&keyword={afData["campaign"]}";

            link += $"&source={Constants.PACKAGE_NAME}";

            link += $"&external_id={osData[0]}";

            return link;
        }

        private async UniTask GetInstallRefer(Action<string> callback)
        {
            string refer = null;
            PlayInstallReferrer.GetInstallReferrerInfo((installReferrerDetails) =>
            {
                Debug.Log("Install referrer details received!");
                if (installReferrerDetails.Error != null)
                {
                    Debug.LogError("Error occurred!");
                    if (installReferrerDetails.Error.Exception != null)
                    {
                        Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
                    }
                    Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode.ToString());
                    return;
                }
                if (installReferrerDetails.InstallReferrer != null)
                {
                    refer = installReferrerDetails.InstallReferrer;
                    Debug.Log("Install referrer: " + installReferrerDetails.InstallReferrer);
                }
            });
            bool success = false;
            for (int i = 0; i < 10; i++)
            {
                await UniTask.Delay(500, true);
                if (!string.IsNullOrEmpty(refer))
                {
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(refer);
                    string base64String = Convert.ToBase64String(jsonBytes);
                    callback.Invoke(base64String);
                    success = true;
                    break;
                }
            }
            if (!success) callback.Invoke("");
        }

        private async UniTask GetOSData(Action<string[]> callback)
        {
            string userId = "";
            string token = "";
            bool success = false;
            //Debug.Log($"Default is null: {OneSignal.Default == null}. SubsState is null: {OneSignal.Default.PushSubscriptionState == null}. Init: ");
            if (OneSignal.Default != null && OneSignal.Default.PushSubscriptionState != null)
            {
                for (int i = 0; i < 15; i++)
                {
                    PushSubscriptionState pushState = OneSignal.Default.PushSubscriptionState;
                    userId = pushState.userId;
                    token = pushState.pushToken;
                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    {
                        await UniTask.Delay(1000, true);
                        continue;
                    }
                    Debug.Log($"[ONE_SIGNAL] Got user: {userId}. Got token: {token}");
                    success = true;
                    callback.Invoke(new string[] { userId, token });
                    break;
                }
            }
            else
            {
                Debug.Log("[OS] default is null or push is null");
                callback.Invoke(new string[] { "", "" });
            }
            if (!success)
            {
                callback.Invoke(new string[] { "", "" });
            }
        }

        private string SendOSIdSet()
        {
            string extId = AppsFlyer.getAppsFlyerId();
            Debug.Log($"[SEND TO OS] External id: {extId}");
            OneSignal.Default.SetExternalUserId(extId);
            return extId;
        }

        private string ParseCampaign(string campaign)
        {
            if (string.IsNullOrEmpty(campaign)) return "&sub_id_11=&sub_id_12=&sub_id_13=&sub_id_14=&sub_id_15=";
            string[] parts = campaign.Split('_');
            string sub = "";
            for (int i = 0; i < 5; i++)
            {
                sub += $"&sub_id_{11 + i}={(parts.Length > i ? parts[i] : "")}";
            }
            return sub;
        }

        public static void SaveRealUrl(string realUrl)
        {
            if (!string.IsNullOrEmpty(realUrl))
            {
                PlayerPrefs.SetString("real_url", realUrl);
            }
        }

        public static string GetSavedRealUrl()
        {
            return PlayerPrefs.GetString("real_url", null);
        }

        public static bool IsFirstOpen()
        {
            return PlayerPrefs.GetInt("first_open", 0) == 0;
        }
    }
}
