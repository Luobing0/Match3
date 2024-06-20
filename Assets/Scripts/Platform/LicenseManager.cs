using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class LicenseManager : MonoBehaviour
{
    public static LicenseManager instance;
    //public string activeUrl = "http://1.116.220.18:8080/SMARTITANAPP/AppService";
    string activeUrl = "http://1.116.220.18/SMARTITANAPP/AppService";
    public Button btn;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PostVerifyLicenseMsg(snNum, appCodeStr);

        //PostActiveLicenseMsg("123456789");
        
        /* DateTime date = DateTime.Now.ToLocalTime(); */ //当前系统时间
        string date = "2024-01-22";
        string data1 = "2024-01-22";       // "/" 跟 "-" 效果是一样的
        //string data3 = "2021-01-01 12:12:12";
        //CompanyDate(date.ToString(), data1, ref msg);
         CompanyDate(date, data1, ref msg);
        btn.onClick.AddListener(()=> { PostVerifyLicenseMsg(snNum, appCodeStr); });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PostVerifyLicenseMsg(snNum, appCodeStr);
        }
    }
    string snNum = "8135567616283550";
    //string snNum = "3381170773293670";
    string appCodeStr = "JPHY_RENJIAO";
    /// <summary>
    /// 校验
    /// </summary>
    /// <param name="sn"></param>序列号
    /// <param name="appCode"></param>产品编码
    public void PostVerifyLicenseMsg(string sn,string appCode)
    {
        WWWForm form = new WWWForm();
        form.AddField("FunctionCode", "SNVERIFY");
        form.AddField("serialno", sn);
        form.AddField("appcode", appCode);
        //CNetMgr.Instance.GetMsg(Phone_URL_msg, true, GetData, Phone_Token);
        //string jsonStr = JsonConvert.SerializeObject(ht,Formatting.Indented);
        Debug.Log("sn" + sn + "appcode" + appCode);
        //CNetMgr.Instance.PostMsg(activeUrl, false, jsonStr, OnPostVerifyResponse, null, OnPostVerifyResponseError);
        CNetMgr.Instance.PostMsgForm(activeUrl, false, form, OnPostVerifyResponse, null, OnPostVerifyResponseError);
    }
    [SerializeField]
    // LicenseInfo adLicense;
    public  string msg;
    public Action OnVerifySuccess;
    void OnPostVerifyResponse(string str) 
    {
        Debug.Log("VerifyRes" + str);
        if (str.Contains("GEONORECORD"))
            Debug.Log("序列号不存在或者已经激活");
        else if (str.Contains("GEOFAILIED"))
        {
            Debug.Log("程序错误");
        }
        else if (str.Contains("GEOPMSISNULL"))
        {
            Debug.Log("请求参数有空值");
        }
        else
        {

            Debug.Log("Success");
            OnVerifySuccess?.Invoke();
        }
    }
    void OnPostVerifyResponseError() 
    {
    
    }
    public  void PostActiveLicenseMsg(string sn)
    {
        WWWForm form = new WWWForm();
        form.AddField("FunctionCode", "SNVERIFY");
        form.AddField("serialno", "4077035660232328");
        CNetMgr.Instance.PostMsgForm(activeUrl, false, form, OnPostActiveResponse, null, OnPostActiveResponseError);
    }

    void OnPostActiveResponse(string str)
    {
        Debug.Log("ActRes"+str);
    }
    void OnPostActiveResponseError()
    {

    }
    public void CompanyDate(string dateStr1, string dateStr2, ref string msg)
    {
        //将日期字符串转换为日期对象
        DateTime t1 = Convert.ToDateTime(dateStr1);
        DateTime t2 = Convert.ToDateTime(dateStr2);
        //通过DateTIme.Compare()进行比较（）
        int num = DateTime.Compare(t1, t2);

        //t1> t2
        if (num > 0)
        {
            msg = "t1:(" + dateStr1 + ")大于" + "t2(" + dateStr2 + ")";
            Debug.Log(msg);
        }
        //t1= t2
        if (num == 0)
        {
            msg = "t1:(" + dateStr1 + ")等于" + "t2(" + dateStr2 + ")";
            Debug.Log(msg);
        }
        //t1< t2
        if (num < 0)
        {
            msg = "t1:(" + dateStr1 + ")小于" + "t2(" + dateStr2 + ")";
            Debug.Log(msg);
        }
    }
}
