using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KevinUtility.Net.Models
{
    /// <summary>
    /// 封装的HttpWebResponse
    /// </summary>
    public class FetchedResponse
    {
        public HttpWebResponse Response { get; private set; }

        public HttpStatusCode StatusCode
        {
            get
            {
                return Response != null ? Response.StatusCode : HttpStatusCode.NotFound;
            }
        }

        public FetchedResponse(HttpWebResponse response = null)
        {
            Response = response;
        }

        /// <summary>
        /// 获取FetchedJson 
        /// </summary>
        /// <returns></returns>
        public FetchedJson Json()
        {
            return new FetchedJson(GetResult());
        }

        /// <summary>
        /// 直接获取文本
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            return GetResult();
        }

        private string GetResult()
        {
            if (Response == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var stream = Response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    // LogHelper.Log.Error(ex.Message, ex);
                    return null;
                }
            }
        }
    }

}
