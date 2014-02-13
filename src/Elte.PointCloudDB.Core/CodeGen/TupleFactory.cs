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
    /// <summary>
    /// Implements logic needed to generate tuple structures at runtime.
    /// </summary>
    /// <remarks>
    /// Strongly types structures and functions are generated at runtime
    /// for tuples.
    /// </remarks>
    public sealed class TupleFactory
    {
        /// <summary>
        /// Singleton instance of the tuple factory.
        /// </summary>
        private static readonly TupleFactory instance;

        /// <summary>
        /// Gets the singleton instance of the tuple factory.
        /// </summary>
        public static TupleFactory Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Initializes static members.
        /// </summary>
        static TupleFactory()
        {
            instance = new TupleFactory();
        }

        /// <summary>
        /// Holds a cache of tuple helpers already generated.
        /// </summary>
        private ConcurrentDictionary<string, TupleHelperBase> tupleHelperCache;

        /// <summary>
        /// 
        /// </summary>
        private TupleFactory()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Initializes private member variables.
        /// </summary>
        private void InitializeMembers()
        {
            tupleHelperCache = new ConcurrentDictionary<string, TupleHelperBase>();
        }

        /// <summary>
        /// Gets a tuple helper generated for the column set provided.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        /// <remarks>
        /// The tuple is determined by the data types of the column set entirely,
        /// column names are not used. Helpers and tuple structs can be freely reused
        /// if column types are identical. This function caches tuple helpers and
        /// returns them from the cache if they have already been generated, or
        /// generates them on the fly.
        /// </remarks>
        public TupleHelperBase GetTupleHelper(SchemaObjectCollection<Column> columns)
        {
            // Get the unique name of the tuple type
            var name = GetTupleName(columns);

            TupleHelperBase helper;

            // See if this type of tuple is already in the cache, if not, generate the helper
            // and the tuple struct.
            if (!tupleHelperCache.TryGetValue(name, out helper))
            {
                helper = CreateTupleHelper(name, columns);
                tupleHelperCache.TryAdd(name, helper);
            }

            return helper;
        }

        /// <summary>
        /// Creates a new tuple struct and a tuple helper class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private TupleHelperBase CreateTupleHelper(string name, SchemaObjectCollection<Column> columns)
        {
            // Create the tuple struct
            var tupleType = CreateTupleStruct(name, columns);

            // Create generic type fo the tuple helper and intantiate helper class
            var helperType = typeof(TupleHelper<>).MakeGenericType(tupleType);
            var helper = (TupleHelperBase)Activator.CreateInstance(helperType);

            // Initialize columns and create column parsers
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
        /// <remarks>
        /// The function uses CodeDom classes to assemble a struct that will represent
        /// a tuple determined by the column set. The struct is compiled at runtime
        /// and loaded into the current AppDomain.
        /// </remarks>
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

            // Generate fields for each column
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

            // Create a C# compiler
            var provider = CodeDomProvider.CreateProvider("cs");
            var par = new CompilerParameters()
            {
                GenerateInMemory = true,
#if DEBUG
                IncludeDebugInformation = true,
#endif
            };

            // Compile the tuple struct and load the compiled type
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

        /// <summary>
        /// Returns a namespace for generated code.
        /// </summary>
        /// <returns></returns>
        public CodeNamespace GetGeneratedNamespace()
        {
            return new CodeNamespace(typeof(Server).Namespace + ".Generated");
        }
    }
}
