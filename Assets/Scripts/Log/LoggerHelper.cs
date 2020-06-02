using UnityEngine;
using System;

namespace agora
{
    namespace unity
    {
        public class LoggerHelper : ILogHelper
        {
            public LOG_LEVEL_TYPE LogLevel 
            {
                get;
                set;
            }

            public string Title
            {
                get;
                set;
            }

            public string FormatInfo
            {
                get;
                set;
            }

            public string FormatWarning
            {
                get;
                set;
            }

            public string FormatError
            {
                get;
                set;
            }

            public LoggerHelper ()
            {
                LogLevel = LOG_LEVEL_TYPE.ALL;
                FormatInfo = "[" + Application.Wayang_Title + "]" + "(" + Tools.GetLogDateTime() + ")" + " Info [{0}]: {1}\r\n";
                FormatWarning = "[" + Application.Wayang_Title + "]" + "(" + Tools.GetLogDateTime() + ")" +  " Warning [{0}]: {1}\r\n";
                FormatError = "[" + Application.Wayang_Title + "]" + "(" + Tools.GetLogDateTime() + ")" +  " Error [{0}]: {1}\r\n";
            }

            public void Info (string tag, string logMessage)
            {
                if (LogLevel <= LOG_LEVEL_TYPE.INFO)
                {
                    Debug.Log(string.Format(FormatInfo, tag, logMessage));
                }
            }
            
            public void Warning (string tag, string logMessage)
            {
                if (LogLevel <= LOG_LEVEL_TYPE.WARNING)
                {
                    Debug.LogWarning(string.Format(FormatWarning, tag, logMessage));
                }
            }

            public void Error (string tag, string logMessage)
            {
                if (LogLevel <= LOG_LEVEL_TYPE.ERROR)
                {
                    Debug.LogError(string.Format(FormatError, tag, logMessage));
                }
            }
        }
    }
}
