using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxelotTask
{
    public class ReportFailedException : Exception
    {
        public ReportFailedException(string message)
        : base(message)
        { }
    }
}
