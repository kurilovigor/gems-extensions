using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gems.Data.Extensions.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class TimeMetricAttribute : Attribute
    {
    }
}
