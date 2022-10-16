using System.Diagnostics;
using UnityEngine;

namespace Util
{
    public static class Debug
    {
        [Conditional("UNITY_EDITOR")]
        public static void Print(object msg) => UnityEngine.Debug.Log(msg);
        
        [Conditional("UNITY_EDITOR")]
        public static void Print(string msg) => UnityEngine.Debug.Log(msg);

        [Conditional("UNITY_EDITOR")]
        public static void PrintColor(string msg, Color color)
        {
            Print($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{msg}</color>");
        }
    }
}