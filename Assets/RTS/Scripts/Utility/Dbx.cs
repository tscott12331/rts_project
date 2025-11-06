using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using UnityEditor;

public class Dbx : UnityEngine.Debug
{
    public static void CtxLog(string log) {
        StackTrace stackTrace = new();

        CtxLog(log, stackTrace);
    }
    public static void CtxLog(string log, StackTrace stackTrace) {
        var mthd = stackTrace.GetFrame(1).GetMethod();
        string mthdName = mthd.Name;
        string className = mthd.ReflectedType.Name;

        UnityEngine.Debug.Log($"[{className}.{mthdName}]: {log}");
    }

    public static void LogCollection<T>(ICollection<T> list, Func<T, string> itemToString)
    {
        StackTrace stackTrace = new();

        if(list == null)
        {
            CtxLog("List is null", stackTrace);
        }

        string str = $"List {list}\n";
        for (int i = 0; i < list.Count; i++)
        {
            var item = list.ElementAtOrDefault(i);
            var itemStr = item != null ? itemToString(item) : "null";
            str += $"{i}: {itemStr}\n";
        }

        CtxLog(str, stackTrace);
    }
}
