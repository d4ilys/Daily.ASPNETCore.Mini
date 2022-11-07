using System.Text.RegularExpressions;

namespace Daily.ASPNETCore.Mini.Common
{
    public static class StringException
    {
        //匹配全字符，验证字符串，验证IPV4
        /// <summary>
        /// 获得完全匹配的正则表达式
        /// </summary>
        /// <param name="regStr"></param>
        /// <returns></returns>
        public static string GetPerfectRegStr(string regStr)
        {
            if (string.IsNullOrEmpty(regStr))
                return string.Empty;
            const char first = '^';
            const char last = '$';
            if (regStr[0] != first)
            {
                regStr = first + regStr;
            }
            if (regStr[regStr.Length - 1] != last)
            {
                regStr += last;
            }
            return regStr;
        }
        /// <summary>
        /// 验证字符串
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="regStr"></param>
        /// <returns></returns>
        public static bool VerifyRegex(this string inputStr, string regStr)
        {
            return !string.IsNullOrEmpty(regStr) && !string.IsNullOrEmpty(inputStr) && Regex.IsMatch(inputStr, regStr);
        }
        /// <summary>
        /// 验证字符串
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="regStr"></param>
        /// <param name="isPerfect"></param>
        /// <returns></returns>
        public static bool VerifyRegex(this string inputStr, string regStr, bool isPerfect)
        {
            regStr = isPerfect ? GetPerfectRegStr(regStr) : regStr;
            return inputStr.VerifyRegex(regStr);
        }
        /// <summary>
        /// 验证字符串是否为IPv4
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsIPv4(this string inputStr)
        {
            return VerifyRegex(inputStr, RegexData.InternetProtcolV4, true);
        }
    }
}
