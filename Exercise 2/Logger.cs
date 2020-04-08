using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyService
{
    public class Logger
    {
        private static int nextFlowId = 1;

        public string Context { private set; get; }

        public int FlowId { private set; get; }

        public Logger(string context)
        {
            this.Context = context;
            this.FlowId = nextFlowId++;
        }

        public Logger(string context, int flowId)
        {
            this.Context = context;
            this.FlowId = flowId;
        }

        public void Write(string msg)
        {
            Console.WriteLine($" -- {this.FlowId} -- [{Context}] {msg}");
        }

        public static void ResetNextLoggerId()
        {
            nextFlowId = 1;
        }
    }
}
