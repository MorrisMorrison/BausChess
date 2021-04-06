using System;
using System.Diagnostics;

namespace BausChess.Utils
{
    public static class DebugHelper
    {
        private static bool _debug = true;
        
        public static void DebugPrint(string message)
        {
            if (_debug)
            {
                Debug.Print(message);
            }            
        }
        
        public static void ConsolePrint(string message)
        {
            if (_debug)
            {
                Console.WriteLine(message);
            }            
        }
    }
}