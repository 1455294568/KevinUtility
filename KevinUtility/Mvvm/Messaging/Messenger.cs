using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KevinUtility.Mvvm
{
    public class Messenger : IMessenger
    {
        private static object _lock = new object();
        private static object _registerLock = new object();
        private static Messenger instance;
        private Dictionary<Type, List<ActionAndToken>> _dictActions;
        private bool _isCleanupRegistered;

        private Messenger() 
        {
            _dictActions = new Dictionary<Type, List<ActionAndToken>>();
        }

        public static Messenger Instance
        { 
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new Messenger();
                        }
                    }
                }
                return instance;
            }
        }

        public void Register<TMessage>(object receiver, Action<TMessage> action)
        {
            Register(receiver, null, action);
        }

        public void Register<TMessage>(object receiver, object token, Action<TMessage> action)
        {
            var type = typeof(TMessage);
            lock (_registerLock)
            {
                lock (_dictActions)
                {
                    List<ActionAndToken> list;
                    if (!_dictActions.ContainsKey(type))
                    {
                        list = new List<ActionAndToken>();
                        _dictActions.Add(type, list);
                    }
                    else
                    {
                        list = _dictActions[type];
                    }

                    ActionAndToken actionInfo = new ActionAndToken
                    {
                        ActionP = action,
                        Receiver = receiver,
                        Token = token
                    };
                    list.Add(actionInfo);
                }
            }

            RequestCleanup();
        }

        public void Send<TMessage>(TMessage message)
        {
            SendToTargetOrType(message, null, null);
        }

        public void Send<TMessage, TTarget>(TMessage message)
        {
            SendToTargetOrType(message, typeof(TTarget), null);
        }

        public void Send<TMessage>(TMessage message, object token)
        {
            SendToTargetOrType(message, null, token);
        }

        public void Unregister(object receiver)
        {
            UnregisterFromLists(receiver, _dictActions);
        }

        public void Unregister<TMessage>(object receiver)
        {
            Unregister<TMessage>(receiver, null, null);
        }

        public void Unregister<TMessage>(object receiver, Action<TMessage> action)
        {
            Unregister(receiver, null, action);
        }

        public void Unregister<TMessage>(object receiver, object token)
        {
            Unregister<TMessage>(receiver, token, null);
        }

        public void Unregister<TMessage>(object receiver, object token, Action<TMessage> action)
        {
            UnregisterFromLists(receiver, token, action, _dictActions);
            RequestCleanup();
        }

        public void SendToTargetOrType<TMessage>(TMessage message, Type targetType,object token)
        {
            var listClone = _dictActions.Keys.Take(_dictActions.Count).ToList();
            var type = typeof(TMessage);

            foreach (var i in listClone)
            {
                List<ActionAndToken> list = null;
                if (type == i)
                {
                    lock (_dictActions)
                    {
                        var actionInfo = _dictActions[i];
                        list = actionInfo.Take(actionInfo.Count).ToList();
                    }
                }
                SendToList(message, list, targetType, token);
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        private void SendToList<TMessage>(TMessage message, IEnumerable<ActionAndToken> actionList, Type messageType, object token)
        {
            if (actionList != null)
            {
                var listClone = actionList.ToList().Take(actionList.Count()).ToList();

                foreach (var i in listClone)
                {
                    if (i.ActionP != null 
                        && (messageType == null 
                            || messageType == i.Receiver.GetType() 
                            || messageType.IsAssignableFrom(i.Receiver.GetType())) 
                        && ((token == null && i.Token == null) 
                            || i.Token != null && i.Token.Equals(token)))
                    {
                        i.ActionP.DynamicInvoke(new object[] { message });
                    }
                }
            }
        }

        private static void UnregisterFromLists(object receiver, Dictionary<Type, List<ActionAndToken>> lists)
        {
            if (receiver == null
                || lists == null
                || lists.Count == 0)
            {
                return;
            }

            lock (lists)
            {
                foreach (var messageType in lists.Keys)
                {
                    foreach (var item in lists[messageType])
                    {

                        if (item.ActionP != null
                            && receiver == item.Receiver)
                        {
                            item.Delete();
                        }
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(
            object receiver,
            object token,
            Action<TMessage> action,
            Dictionary<Type, List<ActionAndToken>> lists)
        {
            var messageType = typeof(TMessage);

            if (receiver == null
                || lists == null
                || lists.Count == 0
                || !lists.ContainsKey(messageType))
            {
                return;
            }

            lock (lists)
            {
                foreach (var item in lists[messageType])
                {

                    if (item.ActionP != null
                        && receiver == item.Receiver
                        && (action == null
                        && (token == null
                            || token.Equals(item.Token))))
                    {
                        item.Delete();
                    }
                }
            }
        }

        public void Cleanup()
        {
            CleanUpList(_dictActions);
            _isCleanupRegistered = false;
        }

        public void RequestCleanup()
        {
            if (!_isCleanupRegistered)
            {
                Action cleanupAction = Cleanup;
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    cleanupAction,
                    DispatcherPriority.ApplicationIdle,
                    null);
                _isCleanupRegistered = true;
            }

        }

        private void CleanUpList(Dictionary<Type, List<ActionAndToken>> lists)
        {
            lock (lists)
            {
                var listToRemove = new List<Type>();
                foreach (var list in lists)
                {
                    var removableList = list.Value.Where(s => s.ActionP == null).ToList();
                    foreach (var removable in removableList)
                    {
                        list.Value.Remove(removable);
                    }

                    if (list.Value.Count == 0)
                    {
                        listToRemove.Add(list.Key);
                    }
                }

                foreach (var removable in listToRemove)
                {
                    lists.Remove(removable);
                }

            }
        }

        public struct ActionAndToken
        {
            public Delegate ActionP;

            public object Receiver;

            public object Token;

            public void Delete()
            {
                ActionP = null;
            }
        }
    }
}
