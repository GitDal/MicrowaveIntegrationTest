using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microwave.Test.Integration.UtilityMethods
{
    public static class TextWriterHelper
    {
        // Utility method
        public static void ClearTextWriter(StringWriter tr)
        {
            StringBuilder sb = tr.GetStringBuilder();
            sb.Remove(0, sb.Length);
        }
    }
}
