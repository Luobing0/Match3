
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class CNetMgr : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
   public static CNetMgr Instance;
    private void Awake()
    {
       Instance = this;
    }
    /// <summary>
    /// 发送到服务器消息并返回
    /// </summary>
    /// <param name="url">服务器URL链接</param>
    /// <param name="haveToken">是否带有token</param>
    /// <param name="json">json字符串</param>
    /// <param name="action">服务器返回数据</param>
    /// <param name="token">token识别码</param>
    public void PostMsg(string url, bool haveToken, string json, Action<string> action,string token = null, Action Erroraction=null)
    {
        StartCoroutine(POST(url, haveToken, token, json, action,Erroraction));
    }
    public void PostMsgForm(string url, bool haveToken, WWWForm form, Action<string> action, string token = null, Action Erroraction = null)
    {
        StartCoroutine(POSTFORM(url, haveToken, token, form, action, Erroraction));
    }

    /// <summary>
    /// 获取服务器数据
    /// </summary>
    /// <param name="url">服务的URL</param>
    /// <param name="action">服务器返回的数据</param>
    public void GetMsg(string url, bool haveToken, Action<string> action, string token = null)
    {
        StartCoroutine(GET(url, haveToken, action, token));
    }
    public void DelMsg(string url, bool haveToken, Action<string> action, string token = null)
    {
        StartCoroutine(Del(url, haveToken, action, token));
    }
    /// <summary>
    /// 发送到服务器消息并返回
    /// </summary>
    /// <param name="url">服务器URL链接</param>
    /// <param name="haveToken">是否带有token</param>
    /// <param name="token">token识别码</param>
    /// <param name="json">json字符串</param>
    /// <param name="action">服务器返回数据</param>
    private IEnumerator POST(string url, bool haveToken, string token, string json,Action<string> action, Action Erroraction = null)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(byteArray);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            if (haveToken)
            {
                webRequest.SetRequestHeader("Authorization", token);
            }
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(url+"  posT "+webRequest.error);
                if (Erroraction != null)
                    Erroraction();
            }
            else
            {
                Debug.Log(url+"HTTP响应数据：" + webRequest.downloadHandler.text);
                if (action != null)
                    action(webRequest.downloadHandler.text);
            }
        }
          
       
    }
    private IEnumerator POSTFORM(string url, bool haveToken, string token, WWWForm form, Action<string> action, Action Erroraction = null)
    {
       
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.uploadHandler.Dispose();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form post complete!"+www.downloadHandler.text);
            if(action!=null)
               action(www.downloadHandler.text);
        }
        // 保险起见再设置一下handler随WebRequest一起释放
        www.disposeUploadHandlerOnDispose = true;
        www.disposeDownloadHandlerOnDispose = true;
        www.disposeCertificateHandlerOnDispose = true;
    }
    /// <summary>
    /// 获取服务器数据
    /// </summary>
    /// <param name="url">服务的URL</param>
    /// <param name="action">服务器返回的数据</param>
    private IEnumerator GET(string url,bool haveToken, Action<string> action, string token = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            if (haveToken)
            {
                webRequest.SetRequestHeader("Authorization", token);
            }
            yield return webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(url+"==="+webRequest.downloadHandler.text);
                if(action!=null)
                action(webRequest.downloadHandler.text);
            }
        }
    }

    private IEnumerator Del(string url, bool haveToken, Action<string> action, string token = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            if (haveToken)
            {
                webRequest.SetRequestHeader("Authorization", token);
            }
            yield return webRequest.SendWebRequest();
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.Log(webRequest.error);
            }
            else
            {
               // Debug.Log(webRequest.downloadHandler.text);
               // if (action != null)
                  //  action(webRequest.downloadHandler.text);
            }
        }
    }
}
