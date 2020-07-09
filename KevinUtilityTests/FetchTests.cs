using Microsoft.VisualStudio.TestTools.UnitTesting;
using KevinUtility.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace KevinUtility.Net.Tests
{
    [TestClass()]
    public class FetchTests
    {
        [TestMethod()]
        public void RequestTest()
        {
            var str = Fetch.Request(
               url: "http://127.0.0.1:8080/api/goods/UpdateCheckStatus",
               method: "POST",
               header: new Dictionary<string, object>
               {
                    { "Content-Type", "application/json" },
                    { "Authorization", "123" }
               },
               body: JsonHelper.SerializeObject( new { CheckWeighter = false })
           ).Then(s =>
           {
               if (s.StatusCode == HttpStatusCode.OK)
               {
                   return s.Json();
               }
               else return null;
           }).Done();
            Console.WriteLine(str);
            Assert.IsNotNull(str);
            Assert.AreNotEqual(string.Empty, str);
        }
    }
}