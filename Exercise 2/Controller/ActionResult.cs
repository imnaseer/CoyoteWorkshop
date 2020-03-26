using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyService
{
    public class ActionResult<T>
    {
        public bool Success;
        public T Response;
    }
}
