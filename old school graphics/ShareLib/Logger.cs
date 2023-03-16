using System;
using System.Diagnostics;

namespace ShareLib
{
    static public class Logger
    {
        public static void Log(string msg)
        {
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }
    }
}
