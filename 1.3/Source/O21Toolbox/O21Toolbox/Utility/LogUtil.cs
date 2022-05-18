using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox
{
    public static class LogUtil
    {
        public static readonly Color msgColor = new Color(0.266f, 0.580f, 0.890f);

        public static readonly Color wrnColor = new Color(0.796f, 0.325f, 0.878f);

        public static readonly Color errColor = new Color(0.901f, 0.192f, 0.152f);

        public static readonly string msgPrefix = ":: Toolbox ::";

        public static void LogMessage(string msg)
        {
            if (Log.ReachedMaxMessagesLimit)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Message, $"{msgPrefix.Colorize(msgColor)} {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }

        public static void LogWarning(string msg)
        {
            if (Log.ReachedMaxMessagesLimit)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Message, $"{msgPrefix.Colorize(wrnColor)} {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }

        public static void LogError(string msg)
        {
            if (Log.ReachedMaxMessagesLimit)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Message, $"{msgPrefix.Colorize(errColor)} {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }
    }
}
