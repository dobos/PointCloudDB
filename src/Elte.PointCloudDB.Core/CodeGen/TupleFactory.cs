using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Elte.PointCloudDB.Schema;

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

        private ConcurrentDictionary<string, TupleHelperBase> tupleHelperCache;

        private TupleFactory()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            tupleHelperCache = new ConcurrentDictionary<string, TupleHelperBase>();
        }

        public TupleHelperBase GetTupleHelper(SchemaObjectCollection<Column> columns)
        {
            var name = GetTupleName(columns);

            TupleHelperBase helper;

            if (!tupleHelperCache.TryGetValue(name, out helper))
            {
                helper = CreateTupleHelper(name, columns);
                tupleHelperCache.TryAdd(name, helper);
            }

            return helper;
        }

        public Type GetTupleStuctType(SchemaObjectCollection<Column> columns)
        {
            return GetTupleHelper(columns).GetTupleType();
        }

        private TupleHelperBase CreateTupleHelper(string name, SchemaObjectCollection<Column> columns)
        {
            var tupleType = CreateTupleStruct(name, columns);
            var helperType = typeof(TupleHelper<>).MakeGenericType(tupleType);

            // Instantiate helper class
            var helper = (TupleHelperBase)Activator.CreateInstance(helperType);

            // Initialize columns
            helper.SetColumns(columns);
            for (int i = 0; i < columns.Count; i++)
            {
                helper.SetParseColumnValueDelegate(i, CreateColumnParserDelegate(tupleType, i));
            }

            return helper;
        }

        /// <summary>
        /// Generates code that implements the tuple columns as a struct
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private Type CreateTupleStruct(string name, SchemaObjectCollection<Column> columns)
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

            for (int i = 0; i < columns.Count; i++)
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

        /// <summary>
        /// Creates a function that parses a string into a column value
        /// </summary>
        /// <param name="tupleType"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private Delegate CreateColumnParserDelegate(Type tupleType, int i)
        {
            // Delegate type
            var delegateType = typeof(ColumnParserDelegate<>).MakeGenericType(tupleType);

            // First create the function parameters
            var data = Expression.Parameter(tupleType.MakeByRefType(), "data");
            var value = Expression.Parameter(typeof(string), "value");

            // Column parser code
            var fields = tupleType.GetFields();

            var field = Expression.Field(data, fields[i]);
            var parser = fields[i].FieldType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var parse = Expression.Call(parser, value);
            var assign = Expression.Assign(field, parse);

            var lambda = Expression.Lambda(delegateType, assign, new ParameterExpression[] { data, value });

            var fn = lambda.Compile();

            return fn;
        }

        /// <summary>
        /// Generates tuple name from column types.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetTupleName(SchemaObjectCollection<Column> columns)
        {
            var name = "__tuple";

            for (int i = 0; i < columns.Count; i++)
            {
                name += "_" + columns[i].DataType.ID;
            }

            return name;
        }

        private CodeNamespace GetGeneratedNamespace()
        {
            return new CodeNamespace(typeof(Server).Namespace + ".Generated");
        }
    }
}
