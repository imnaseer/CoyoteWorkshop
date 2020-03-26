using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyService
{
    public class Logger
    {
        private string context;

        public Logger(string context)
        {
            this.context = context;
        }

        public void Write(string msg)
        {
            Console.WriteLine($"[{context}] {msg}");
        }
    }
}
