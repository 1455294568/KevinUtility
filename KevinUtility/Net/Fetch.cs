using KevinUtility.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        }

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="user_Agent"></param>
        /// <param name="timeOut"></param>
        /// <returns>获得一个AwaitableResponse task</returns>
        public static AwaitableResponse Request(string url
            , string method
            , Dictionary<string, object> header = null
            , object body = null
            , string user_Agent = null
            , int timeOut = 1000)
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
                        request.ContentLength = Encoding.UTF8.GetByteCount(body.ToString());

                        var connectTask = request.GetRequestStreamAsync();
                        if (!connectTask.Wait(timeOut))
                        {
                            // LogHelper.Log.Error("Http connect timeout.");
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
                    // LogHelper.Log.Error("Http post json error", ex);
                    return new FetchedResponse();
                }
            });

            AwaitableResponse res = new AwaitableResponse(task);
            return res;
        }
    }
}
