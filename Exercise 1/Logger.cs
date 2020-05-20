using System;
using System.Threading;

namespace TinyService
{
    public class Logger
    {
        private static int nextFlowId = 1;
        private AsyncLocal<bool> shouldSuppressExceptionLogs = new AsyncLocal<bool>();

        public string Context { private set; get; }

        public int FlowId { private set; get; }

        public Logger(string context)
        {
            this.Context = context;
            this.FlowId = nextFlowId++;
            shouldSuppressExceptionLogs.Value = false;
        }

        public Logger(string context, int flowId)
        {
            this.Context = context;
            this.FlowId = flowId;
            shouldSuppressExceptionLogs.Value = false;
        }

        public void Write(string msg)
        {
            Console.WriteLine($" -- {this.FlowId} -- [{Context}] {msg}");
        }

        public void WriteException(string msg)
        {
            if (!shouldSuppressExceptionLogs.Value)
            {
                Console.Write($"-- {this.FlowId} -- ");
                WriteInColor($"Exception [{Context}] {msg}", ConsoleColor.Yellow);
                Console.WriteLine();
            }
        }

        public static void ResetNextLoggerId()
        {
            nextFlowId = 1;
        }

        public void SuppressExceptionLogging(bool suppress = true)
        {
            shouldSuppressExceptionLogs.Value = suppress;
        }

        private void WriteInColor(string msg, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg);
            Console.ForegroundColor = originalColor;

        }
    }
}
