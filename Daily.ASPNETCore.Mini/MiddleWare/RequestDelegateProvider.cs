using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.HttpContexts;

namespace Daily.ASPNETCore.Mini.MiddleWare
{
    public class RequestDelegateProvider
    {
        public static List<Func<RequestDelegate>> RequestDelegateFuncs { get; set; } =
            new List<Func<RequestDelegate>>();

        public static RequestDelegate CreateRequestDelegate()
        {
            return RequestDelegateFuncs.FirstOrDefault().Invoke();
        }
    }
}