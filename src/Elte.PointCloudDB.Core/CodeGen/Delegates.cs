using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.CodeGen
{
    public delegate void ColumnParserDelegate<T>(ref T data, string value)
        where T : struct;
}
