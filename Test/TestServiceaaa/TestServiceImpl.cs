using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.Service.TestServiceaaa;

public class TestServiceImpl : ITestService
{
    public void ContructorInjectTest()
    {
        Console.WriteLine("构造函数注入测试成功");
    }

    public void PropertInjectTest()
    {
        Console.WriteLine("属性注入测试成功");
    }
}