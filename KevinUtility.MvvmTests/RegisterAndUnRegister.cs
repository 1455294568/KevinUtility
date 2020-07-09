using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KevinUtility.Mvvm;

namespace KevinUtility.MvvmTests
{
    [TestClass]
    public class RegisterAndUnRegister
    {
        [TestMethod]
        public void Register()
        {
            var result = "666";
            string messageStr = null;
            Messenger.Reset();

            Messenger.Instance.Register<TestMessage>(this, (m) => {
                var messagEntity = m as TestMessage;
                if (m != null)
                {
                    messageStr = messagEntity.Test;
                }
                return;
            });

            Assert.AreEqual(null, messageStr);

            Messenger.Instance.Send<TestMessage>(new TestMessage(result));
            Assert.AreEqual(result, messageStr);
        }

        [TestMethod]
        public void RegisterWithToken()
        {
            var token = "hahaha";
            var result = "666";
            string messageStr = null;
            string messageStr2 = null;
            Messenger.Reset();

            Messenger.Instance.Register<TestMessage>(this, token, (m) => {
                var messagEntity = m as TestMessage;
                if (m != null)
                {
                    messageStr = messagEntity.Test;
                }
                return;
            });

            Messenger.Instance.Register<TestMessage>(this, "randomToken", (m) => {
                var messagEntity = m as TestMessage;
                if (m != null)
                {
                    messageStr2 = messagEntity.Test;
                }
                return;
            });


            Assert.AreEqual(null, messageStr);
            Assert.AreEqual(null, messageStr2);

            Messenger.Instance.Send<TestMessage>(new TestMessage(result), token);

            Assert.AreEqual(result, messageStr);
            Assert.AreEqual(null, messageStr2);
        }

        [TestMethod]
        public void RegisterTwoDiffType()
        {
            var result = "666";
            var result2 = "888";
            string messageStr = null;
            string messageStr2 = null;
            Messenger.Reset();

            Messenger.Instance.Register<TestMessage>(this, (m) => {
                var messagEntity = m as TestMessage;
                if (m != null)
                {
                    messageStr = messagEntity.Test;
                }
                return;
            });

            Messenger.Instance.Register<TestMessage2>(this, (m) => {
                var messagEntity = m as TestMessage2;
                if (m != null)
                {
                    messageStr2 = messagEntity.Test;
                }
                return;
            });


            Assert.AreEqual(null, messageStr);
            Assert.AreEqual(null, messageStr2);

            Messenger.Instance.Send<TestMessage>(new TestMessage(result));

            Assert.AreEqual(result, messageStr);
            Assert.AreEqual(null, messageStr2);

            Messenger.Instance.Send<TestMessage2>(new TestMessage2(result2));
            Assert.AreEqual(result2, messageStr2);
        }

        public class TestMessage
        {
            public TestMessage(string str)
            {
                Test = str;
            }

            public string Test { get; set; }
        }

        public class TestMessage2
        {
            public TestMessage2(string str)
            {
                Test = str;
            }

            public string Test { get; set; }
        }
    }
}
