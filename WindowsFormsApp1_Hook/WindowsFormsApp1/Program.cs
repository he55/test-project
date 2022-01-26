using PlayHooky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {

        //We take the TagetClass "this" as our first argument. Don't forget this, it's important!
        //Note that this method is static. This is important.
        public static string addhook(object t, string str)
        {
            manager2.Unhook(methodInfo2);
            object v = methodInfo2.Invoke(t, new object[] { str });
            manager2.Hook(methodInfo2, typeof(Program).GetMethod("addhook"));

            string cn = "Hook";
            if (str.Contains("Window"))
                return cn;
            else
                return cn +"."+ str;
        }

      static  System.Reflection.MethodInfo methodInfo2;
       static HookManager manager2;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Create the HookManager -- make sure this is done thread safe!
            HookManager manager = new HookManager();
            manager2 = manager;

            Type type = typeof(System.Windows.Forms.NativeWindow);
            string assemblyQualifiedName = type.AssemblyQualifiedName;

            Type type1 = Type.GetType("System.Windows.Forms.NativeWindow+WindowClass, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            System.Reflection.MethodInfo methodInfo = type1.GetMethod("GetFullClassName", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Instance);
            methodInfo2 = methodInfo;

            //Hook our target method
            manager.Hook(methodInfo, typeof(Program).GetMethod("addhook"));

            //Unhook the target method
            //manager.Unhook(methodInfo);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
