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
    }
}
