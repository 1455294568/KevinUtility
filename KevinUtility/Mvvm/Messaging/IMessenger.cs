using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinUtility.Mvvm
{
    public interface IMessenger
    {
        void Register<TMessage>(object receiver, Action<TMessage> action);

        void Register<TMessage>(object receiver, object token, Action<TMessage> action);

        void Send<TMessage>(TMessage message);

        void Send<TMessage>(TMessage message, object token);

        void Send<TMessage, TTarget>(TMessage message);

        void Unregister(object receiver);

        void Unregister<TMessage>(object receiver);

        void Unregister<TMessage>(object receiver, object token);

        void Unregister<TMessage>(object receiver, Action<TMessage> action);

        void Unregister<TMessage>(object receiver, object token, Action<TMessage> action);
    }
}
