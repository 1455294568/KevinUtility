using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinUtility.Net.Models
{
    public class AwaitableResponse : AwaitableResult<FetchedResponse>
    {
        public AwaitableResponse(Task<FetchedResponse> _task)
            : base(_task)
        {

        }
    }
}
