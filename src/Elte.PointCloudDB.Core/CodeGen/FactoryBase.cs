using Elte.PointCloudDB.Schema;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.CodeGen
{
    public abstract class FactoryBase
    {

        /// <summary>
        /// Returns a namespace for generated code.
        /// </summary>
        /// <returns></returns>
        protected CodeNamespace GetGeneratedNamespace()
        {
            return new CodeNamespace(typeof(Server).Namespace + ".Generated");
        }

        /// <summary>
        /// Generates type name from column types.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        protected string GetTypeNameBasedOnColumns(string initialName, SchemaObjectCollection<Column> columns)
        {
            var name = initialName;

            for (int i = 0; i < columns.Count; i++)
            {
                name += "_" + columns[i].DataType.ID;
            }

            return name;
        }
    }
}
