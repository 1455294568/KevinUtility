using KevinUtility.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static KevinUtility.DebugUtil;

namespace KevinUtility.Net
{
    /// <summary>
    /// 仿Javascript Fetch
    /// </summary>
    public static class Fetch
    {
        public static int timeOut { get; set; }

        static Fetch()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
        }

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="url">地址 (http:// | https://)</param>
        /// <param name="method">GET | POST</param>
        /// <param name="header">header 头</param>
        /// <param name="body">json需要自行序列化</param>
        /// <param name="user_Agent">User-Agent</param>
        /// <param name="timeOut">请求超时</param>
        /// <returns>获得一个AwaitableResponse task</returns>
        public static AwaitableResponse Request(string url
            , string method
            , Dictionary<string, object> header = null
            , object body = null
            , string user_Agent = null
            , int timeOut = 2000)
        {
            var task = Task.Factory.StartNew<FetchedResponse>(() =>
            {
                try
                {
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.ReadWriteTimeout = timeOut;
                    request.Timeout = timeOut;
                    request.Method = method.ToUpper();
                    request.UserAgent = user_Agent != null ? user_Agent : request.UserAgent;

                    if (header != null)
                    {
                        foreach (var i in header)
                        {
                            if (i.Key == "Content-Type")
                            {
                                request.ContentType = i.Value.ToString();
                            }
                            else
                            {
                                request.Headers.Add(i.Key, i.Value.ToString());
                            }
                        }
                    }

                    if (body != null)
                    {
                        // request.ContentLength = Encoding.UTF8.GetByteCount(body.ToString());

                        var connectTask = request.GetRequestStreamAsync();
                        if (!connectTask.Wait(timeOut))
                        {
                            
                            Log("Http connect timeout.");
                            return new FetchedResponse();
                        }

                        using (var streamWriter = new StreamWriter(connectTask.Result, Encoding.UTF8))
                        {
                            streamWriter.Write(body.ToString());
                        }
                    }

                    var response = request.GetResponse() as HttpWebResponse;
                    return new FetchedResponse(response);
                }
                catch (Exception ex)
                {
                    Log("Http post json error: {0}", ex.StackTrace);
                    return new FetchedResponse();
                }
            });

            AwaitableResponse res = new AwaitableResponse(task);
            return res;
        }

        // 测试例子
        private static async void Test()
        {
            await Fetch.Request(
                url: "http://baidu.com",
                method: "POST",
                body: JsonHelper.SerializeObject(new TestCls() { TestA = "123" }),
                header: new Dictionary<string, object>
                {
                    {"Content-Type", "application/json"}
                },
                user_Agent: "Windows",
                timeOut: 1000
            ).Then(s =>
            {
                if (s.StatusCode == HttpStatusCode.OK)
                {
                    //var ret = s.Text();   直接获取返回值string, Json 和 Text只能调用其中之一, 不然会导致异常
                    var ret = s.Json();
                    Log("response text: {0}", ret.ToString());
                    return ret;
                }
                else
                {
                    return null;
                }
            }).Then(s =>
                s.Entity<TestCls>()
            ).DoneAsync();
        }

        // 测试例子2
        private static void Test2()
        {
            Fetch.Request(
                url: "http://baidu.com",
                method: "POST",
                body: JsonHelper.SerializeObject(new TestCls() { TestA = "123" }),
                header: new Dictionary<string, object>
                {
                    {"Content-Type", "application/json"}
                },
                user_Agent: "Windows",
                timeOut: 1000
            ).Then(s =>
            {
                if (s.StatusCode == HttpStatusCode.OK)
                {
                    //var ret = s.Text();   直接获取返回值string, Json 和 Text只能调用其中之一, 不然会导致异常
                    var ret = s.Json();
                    Log("response text: {0}", ret.ToString());
                    return ret;
                }
                else
                {
                    return null;
                }
            }).Then(s =>
                s.Entity<TestCls>()
            ).Done();
        }

        private class TestCls
        {
            public string TestA { get; set; }
        }
    }
}
