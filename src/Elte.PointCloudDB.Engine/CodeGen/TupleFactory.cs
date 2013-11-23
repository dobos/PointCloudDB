using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Elte.PointCloudDB.CodeGen
{
    public sealed class TupleFactory
    {
        private static readonly TupleFactory instance;

        public static TupleFactory Instance
        {
            get { return instance; }
        }

        static TupleFactory()
        {
            instance = new TupleFactory();
        }

        private ConcurrentDictionary<string, Type> tupleStructCache;

        private TupleFactory()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            tupleStructCache = new ConcurrentDictionary<string, Type>(); 
        }

        public Type GetTupleStuctType(Schema.Column[] columns)
        {
            Type type;
            var name = GetTupleName(columns);

            if (!tupleStructCache.TryGetValue(name, out type))
            {
                type = GenerateTupleStruct(name, columns);
                tupleStructCache.TryAdd(name, type);
            }

            return type;
        }


        /// <summary>
        /// Generate tuple name from column types.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetTupleName(Schema.Column[] columns)
        {
            var name = "__tuple";

            for (int i = 0; i < columns.Length; i++)
            {
                name += "_" + columns[i].DataType.ID;
            }

            return name;
        }

        private CodeNamespace GetGeneratedNamespace()
        {
            return new CodeNamespace(typeof(Server).Namespace + ".Generated");
        }

        private Type GenerateTupleStruct(string name, Schema.Column[] columns)
        {
            var unit = new CodeCompileUnit();

            var ns = GetGeneratedNamespace();
            unit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));

            var st = new CodeTypeDeclaration()
            {
                Attributes = MemberAttributes.Public,
                Name = name,
                IsStruct = true
            };
            ns.Types.Add(st);

            for (int i = 0; i < columns.Length; i++)
            {
                var fl = new CodeMemberField()
                {
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference(columns[i].DataType.DotNetType),
                    Name = String.Format("__column_{0}", i),
                };

                st.Members.Add(fl);
            }

            var provider = CodeDomProvider.CreateProvider("cs");
            var par = new CompilerParameters()
            {
                GenerateInMemory = true,
#if DEBUG
                IncludeDebugInformation = true,
#endif
            };

            var res = provider.CompileAssemblyFromDom(par, unit);

            return res.CompiledAssembly.GetType(ns.Name + "." + name);
        }
    }
}
