using System;

namespace Daily.ASPNETCore.Mini.Controllers.HttpAttribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key
        {
            get;
        }
        public RouteAttribute(string key)
        {
            Key = key;
        }
    }
}
