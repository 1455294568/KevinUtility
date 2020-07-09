using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinUtility.Net.Models
{
    /// <summary>
    /// 包装Task 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AwaitableResult<T>
    {
        public Task<T> task { get; set; }

        public AwaitableResult(Task<T> _task)
        {
            task = _task;
        }

        /// <summary>
        /// 下一步, 异步, 装饰
        /// </summary>
        /// <typeparam name="outEntity"></typeparam>
        /// <param name="predict"></param>
        /// <returns></returns>
        public AwaitableResult<outEntity> Then<outEntity>(Func<T, outEntity> predict)
        {
            var task2 = Task.Factory.StartNew<outEntity>(() =>
            {
                task.Wait();
                return predict(task.Result);
            });
            return new AwaitableResult<outEntity>(task2);
        }

        /// <summary>
        /// 异步时调用获得结果
        /// </summary>
        /// <typeparam name="outEntity"></typeparam>
        /// <param name="predict"></param>
        /// <returns></returns>
        public async Task<outEntity> DoneAsync<outEntity>(Func<T, outEntity> predict)
        {
            return predict(await task);
        }

        /// <summary>
        /// 异步时调用获得结果
        /// </summary>
        /// <returns></returns>
        public async Task<T> DoneAsync()
        {
            return await task;
        }

        /// <summary>
        /// 非异步时调用获得结果
        /// </summary>
        /// <typeparam name="outEntity"></typeparam>
        /// <param name="predict"></param>
        /// <returns></returns>
        public outEntity Done<outEntity>(Func<T, outEntity> predict)
        {
            task.Wait();
            return predict(task.Result);
        }

        /// <summary>
        /// 非异步时调用获得结果
        /// </summary>
        /// <returns></returns>
        public T Done()
        {
            task.Wait();
            return task.Result;
        }
    }
}
