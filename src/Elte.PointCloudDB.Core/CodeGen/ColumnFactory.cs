using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.CodeGen
{
    public sealed class ColumnFactory : FactoryBase
    {
        private static readonly ColumnFactory instance;

        public static ColumnFactory Instance
        {
            get { return instance; }
        }

        static ColumnFactory()
        {
            instance = new ColumnFactory();
        }

        private ConcurrentDictionary<string, ColumnHelperBase> columnHelperCache;

        /// <summary>
        /// 
        /// </summary>
        private ColumnFactory()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Initializes private member variables.
        /// </summary>
        private void InitializeMembers()
        {
            columnHelperCache = new ConcurrentDictionary<string, ColumnHelperBase>();
        }

        public ColumnHelperBase GetColumnHelper(SchemaObjectCollection<Column> columns, int chunkSize)
        {
            // Get the unique name of the column chunks
            var name = GetColumnChunksName(columns);

            ColumnHelperBase helper;

            // See if this type of column chunks is already in the cache, if not, generate the helper
            // and the column chunks struct.
            if (!columnHelperCache.TryGetValue(name, out helper))
            {
                helper = CreateColumnHelper(name, columns, chunkSize);
                columnHelperCache.TryAdd(name, helper);
            }

            return helper;
        }

        private ColumnHelperBase CreateColumnHelper(string name, SchemaObjectCollection<Column> columns, int chunkSize)
        {
            var columnType = CreateColumnStruct(name, columns, chunkSize);

            // Get the related tuple struct from tuple helper via cache of TupleFactory
            var tupleType = TupleFactory.Instance.GetTupleHelper(columns).GetTupleType();

            // Create generic type for the column helper and intantiate helper class
            var helperType = typeof(ColumnHelper<,>).MakeGenericType(new Type[] { columnType, tupleType });
            var helper = (ColumnHelperBase)Activator.CreateInstance(helperType);

            // Initialize columns and create column values assigner
            helper.SetColumns(columns);
            for (int i = 0; i < columns.Count; i++)
            {
                helper.SetColumnValuesAssigner(i, CreateColumnValuesAssigner(columnType, tupleType, i));
            }

            return helper;
        }

        private Type CreateColumnStruct(string name, SchemaObjectCollection<Column> columns, int chunkSize)
        {
            var unit = new CodeCompileUnit();

            var ns = GetGeneratedNamespace();
            unit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));

            var st = new CodeTypeDeclaration()
            {
                Attributes = MemberAttributes.Public,
                Name = name
            };
            ns.Types.Add(st);
            
            // Generate fields for each column
            for (int i = 0; i < columns.Count; i++)
            {
                var fl = new CodeMemberField()
                {
                    Attributes = MemberAttributes.Public,
                    Type = new CodeTypeReference(columns[i].DataType.DotNetType.MakeArrayType()),
                    Name = String.Format("__column_{0}", i),
                    InitExpression = new CodeArrayCreateExpression(columns[i].DataType.DotNetType, chunkSize),
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

        private Delegate CreateColumnValuesAssigner(Type columnType, Type tupleType, int columnIndex)
        {
            // Delegate type
            var delegateType = typeof(ColumnValuesAssigner<,>).MakeGenericType(new Type[] { columnType, tupleType });

            // First create the function parameters
            var tuples = Expression.Parameter(tupleType.MakeArrayType(), "tuples");
            var columnChunks = Expression.Parameter(columnType, "columnChunks");
            var chunkSize = Expression.Parameter(typeof(int), "chunkSize");

            // Creating the (generated) method body.
            
            // Declaring variables which is in the body of (generated) method
            var index = Expression.Variable(typeof(int), "index");

            // Creating label for the loop (for the fulfilled exit condition because there is only 'while(true)' loop in the expression tree)
            LabelTarget label = Expression.Label(); // Creating a label to jump to from a loop.

            var currentColumnChunkField = columnType.GetFields()[columnIndex];

            BlockExpression body = Expression.Block(
                // int index;
                new[] { index},
                // index = 0;
                Expression.Assign(index, Expression.Constant(0)),
                // while (true)
                Expression.Loop(
                    // if
                    Expression.IfThenElse(
                    // (index < chunkSize)
                    Expression.LessThan(index, chunkSize),
                    Expression.Block(
                        // columnChunks.'current column fieldname'[index] =
                        Expression.Assign(Expression.ArrayAccess(Expression.Field(columnChunks, currentColumnChunkField), index),
                                            // tuples[index].'tuple fieldname related to current column'
                                            Expression.Field(Expression.ArrayIndex(tuples, index), currentColumnChunkField.Name)), 
                        // index++;
                        Expression.PostIncrementAssign(index)),
                    // else
                    Expression.Break(label, index)
                ),
               label)
            );

            var lambda = Expression.Lambda(delegateType, body, new ParameterExpression[] { tuples, columnChunks, chunkSize });

            var fn = lambda.Compile();

            return fn;
        }

        /// <summary>
        /// Generates tuple name from column types.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetColumnChunksName(SchemaObjectCollection<Column> columns)
        {
            var name = "__columnChunks";

            for (int i = 0; i < columns.Count; i++)
            {
                name += "_" + columns[i].DataType.ID;
            }

            return name;
        }
    }
}
