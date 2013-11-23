using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.Schema;

namespace Elte.PointCloudDB.CodeGen
{
    interface ITupleHelper
    {
        Type GetTupleType();

        void SetColumns(SchemaObjectCollection<Column> columns);
        void SetParseColumnValueDelegate(int i, Delegate parser);
    }
}
