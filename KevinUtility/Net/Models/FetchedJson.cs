using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinUtility.Net.Models
{
    /// <summary>
    /// 封装的Json字符串
    /// </summary>
    public class FetchedJson
    {
        private string Json { get; set; }

        public FetchedJson(string json = null)
        {
            Json = json;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Entity<T>()
        {
            if (Json == null)
            {
                return default(T);
            }
            return JsonHelper.DeSerialize<T>(Json);
        }
    }
}
