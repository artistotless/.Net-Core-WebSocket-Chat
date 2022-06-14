using UnityEngine;
using BestHTTP;
using System;
using System.Text;

public class AuthToken
{
    private Uri _url;

    public AuthToken(string url)
    {
        _url = new Uri(url);
    }

    public void Get(Action<string> callBack)
    {
        var request = new HTTPRequest(_url, HTTPMethods.Post, (req, res) =>
        {
            Debug.Log(_url.ToString());
            Debug.Log(res.StatusCode);
            Debug.Log(res.DataAsText);
            callBack(res.DataAsText);
        });
        request.SetHeader("Content-Type", "application/json");
        request.RawData = Encoding.UTF8.GetBytes($@"{{""name"": ""Irji"",""hardWareInfo"": ""{GetSystemHashCode()}"" }}");
        request.Send();


    }

    private string GetSystemHashCode()
    {
        return (SystemInfo.deviceModel.GetHashCode() ^
             SystemInfo.processorType.GetHashCode() ^
             SystemInfo.operatingSystem.GetHashCode() ^
             SystemInfo.graphicsDeviceID.GetHashCode()).ToString();
    }
}
