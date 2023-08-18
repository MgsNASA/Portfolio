using System;
using UnityEngine;

namespace WWP
{
    public class UWWObject : MonoBehaviour
    {
        public UniWebView Show(string url)
        {
            try
            {
                UniWebView webView = gameObject.AddComponent<UniWebView>();
                webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                webView.OnOrientationChanged += (view, orientation) =>
                {
                    webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                };
                webView.SetZoomEnabled(false);
                webView.Load(url);
                webView.Show();
                webView.OnMultipleWindowOpened += (view, id) => { webView.Load(view.Url); };
                webView.SetSupportMultipleWindows(true, true);
                webView.OnShouldClose += (view) => { return false; };
                return webView;
            }
            catch (Exception)
            {
                //statusText.text += $"\n {ex}";
            }
            return null;
        }
    }
}
