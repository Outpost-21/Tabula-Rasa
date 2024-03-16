using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public static class LogUtil
    {
        public static readonly Color msgColor = new Color(0.266f, 0.580f, 0.890f);

        public static readonly Color wrnColor = new Color(0.796f, 0.325f, 0.878f);

        public static readonly Color errColor = new Color(0.901f, 0.192f, 0.152f);

        public static readonly Color dbgColor = new Color(0.411f, 0.749f, 0.666f);

        public static readonly string msgPrefix = $":: Tabula Rasa ::";

        public static readonly bool debugEnabled = false;

        public static void Message(string msg)
        {
            if (Log.ReachedMaxMessagesLimit)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Message, $"{msgPrefix.Colorize(msgColor)} {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }

        public static void Warning(string msg)
        {
            if (Log.ReachedMaxMessagesLimit)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Warning, $"{msgPrefix.Colorize(wrnColor)} {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }

        public static void Error(string msg)
        {
            if (Log.ReachedMaxMessagesLimit)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Error, $"{msgPrefix.Colorize(errColor)} {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }

        public static void Debug(string msg)
        {
            if (Log.ReachedMaxMessagesLimit || !debugEnabled)
            {
                return;
            }
            UnityEngine.Debug.Log(msg);
            Log.messageQueue.Enqueue(new LogMessage(LogMessageType.Warning, $"{msgPrefix.Colorize(dbgColor)} Debug :: {msg}", StackTraceUtility.ExtractStackTrace()));
            Log.PostMessage();
        }
    }
}
