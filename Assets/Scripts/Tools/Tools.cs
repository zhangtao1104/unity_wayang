using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tools 
{
    public static string GetDateTime()
    {
        DateTime nowDateTime = DateTime.Now;     
        int Year = nowDateTime.Year;
        int Month = nowDateTime.Month;
        int Day = nowDateTime.Day;
        int Hours = nowDateTime.Hour;
        int Minute = nowDateTime.Minute;
        int Second = nowDateTime.Second;
        return Year + "/" + Month + "/" + Day + "   " + Hours + ":" + Minute + ":" + Second;
    }

    public static string GetLogDateTime()
    {
        DateTime nowDateTime = DateTime.Now;
        int Hour = nowDateTime.Hour;
        int Minute = nowDateTime.Minute;
        int Second = nowDateTime.Second;
        long millisecond = nowDateTime.Millisecond;
        return Hour + ":" + Minute + ":" + Second + ":" + millisecond;
    }
}
