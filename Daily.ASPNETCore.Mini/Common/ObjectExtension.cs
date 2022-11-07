using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Daily.ASPNETCore.Mini.Common;

public static class ObjectExtension
{
    private static readonly Dictionary<Type, Func<object, object>> ConvertDictionary = new Dictionary<Type, Func<object, object>>();
    static ObjectExtension()
    {
        ConvertDictionary.Add(typeof(bool), WrapValueConvert(Convert.ToBoolean));
        ConvertDictionary.Add(typeof(bool?), WrapValueConvert(Convert.ToBoolean));
        ConvertDictionary.Add(typeof(int), WrapValueConvert(Convert.ToInt32));
        ConvertDictionary.Add(typeof(int?), WrapValueConvert(Convert.ToInt32));
        ConvertDictionary.Add(typeof(long), WrapValueConvert(Convert.ToInt64));
        ConvertDictionary.Add(typeof(long?), WrapValueConvert(Convert.ToInt64));
        ConvertDictionary.Add(typeof(short), WrapValueConvert(Convert.ToInt16));
        ConvertDictionary.Add(typeof(short?), WrapValueConvert(Convert.ToInt16));
        ConvertDictionary.Add(typeof(double), WrapValueConvert(Convert.ToDouble));
        ConvertDictionary.Add(typeof(double?), WrapValueConvert(Convert.ToDouble));
        ConvertDictionary.Add(typeof(float), WrapValueConvert(Convert.ToSingle));
        ConvertDictionary.Add(typeof(float?), WrapValueConvert(Convert.ToSingle));
        ConvertDictionary.Add(typeof(Guid), m => Guid.Parse(m.ToString()) as object);
        ConvertDictionary.Add(typeof(Guid?), m => Guid.Parse(m.ToString()) as object);
        ConvertDictionary.Add(typeof(string), Convert.ToString);
        ConvertDictionary.Add(typeof(DateTime), WrapValueConvert(Convert.ToDateTime));
        ConvertDictionary.Add(typeof(DateTime?), WrapValueConvert(Convert.ToDateTime));
    }
    /// <summary>
    /// Object转换为Json
    /// </summary>
    /// <param name="inputObj"></param>
    /// <returns></returns>
    public static string ToJson(this object inputObj)
    {
        return JsonConvert.SerializeObject(inputObj);
    }
    /// <summary>
    /// 转换为
    /// </summary>
    /// <param name="inputObj"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static object ConvertTo(this object inputObj, Type targetType)
    {
        if (inputObj == null)
        {
            if (targetType.IsValueType)
                throw new Exception($"不能将null转换为{targetType.Name}");
            return null;
        }
        if (inputObj.GetType() == targetType || targetType.IsInstanceOfType(inputObj))
        {
            return inputObj;
        }
        if (ConvertDictionary.ContainsKey(targetType))
        {
            return ConvertDictionary[targetType](inputObj);
        }
        try
        {
            return Convert.ChangeType(inputObj, targetType);
        }
        catch (Exception ex)
        {
            throw new Exception($"未实现到{targetType.Name}的转换方法", ex);
        }
    }
    #region 私有方法
    /// <summary>
    /// 包装值转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    private static Func<object, object> WrapValueConvert<T>(Func<object, T> input) where T : struct
    {
        return i =>
        {
            if (i == null || i is DBNull)
                return null;
            return input(i);
        };
    }
    #endregion
}