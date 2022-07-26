using System;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Type type1=null;
            Assembly assembly= Assembly.UnsafeLoadFrom(@"C:\Users\luckh\source\repos\ConsoleApp1\ClassLibrary1\bin\Debug\net472\ClassLibrary1.dll");
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (!type.IsAbstract && type.IsPublic && type.IsClass && type.Name== "Person")
                {
                    type1 = type;
                    break;
                }
            }

            object instance = (object)Activator.CreateInstance(type1);
            MethodInfo methodInfo = type1.GetMethod("GetName");
            object v = methodInfo.Invoke(instance, null);
        }
    }
}
