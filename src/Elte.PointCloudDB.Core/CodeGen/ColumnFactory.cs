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

        private const string columnFieldNameFormat = "__column_{0}";

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
            var name = GetTypeNameBasedOnColumns("__columnsChunk", columns);

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
            var dataType = CreateColumnClass(name, columns, chunkSize);

            // Get the related tuple struct from tuple helper via cache of TupleFactory
            var tupleType = TupleFactory.Instance.GetTupleHelper(columns).GetTupleType();

            // Create generic type for the column helper and intantiate helper class
            var helperType = typeof(ColumnHelper<,>).MakeGenericType(new Type[] { dataType, tupleType });
            var helper = (ColumnHelperBase)Activator.CreateInstance(helperType);

            // Initialize columns and create column values assigner
            helper.SetColumns(columns);
            for (int i = 0; i < columns.Count; i++)
            {
                helper.SetColumnAllocatorDelegate(i, CreateColumnAllocatorDelegate(dataType, i));
                helper.SetColumnValuesAssigner(i, CreateColumnValuesAssigner(dataType, tupleType, i));
            }

            return helper;
        }

        private Type CreateColumnClass(string name, SchemaObjectCollection<Column> columns, int chunkSize)
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
                    Name = String.Format(columnFieldNameFormat, i),
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
        /// Creates a function that allocates memory for a column array field.
        /// </summary>
        /// <param name="dataType">the columnsChunk-related class type</param>
        /// <param name="columnIndex">column index</param>
        /// <returns></returns>
        private Delegate CreateColumnAllocatorDelegate(Type dataType, int columnIndex)
        {
            // Delegate type
            var delegateType = typeof(ColumnAllocatorDelegate<>).MakeGenericType(dataType);

            // First create the function parameters
            var data = Expression.Parameter(dataType, "data");
            var chunkSize = Expression.Parameter(typeof(int), "chunkSize");

            // Column allocation code
            var fields = dataType.GetFields();

            var field = Expression.Field(data, fields[columnIndex]);
            var allocation = Expression.NewArrayBounds(fields[columnIndex].FieldType.GetElementType(), chunkSize);
            var body = Expression.Assign(field, allocation);

            var lambda = Expression.Lambda(delegateType, body, new ParameterExpression[] { data, chunkSize });

            var fn = lambda.Compile();

            return fn;
        }

        private Delegate CreateColumnValuesAssigner(Type dataType, Type tupleType, int columnIndex)
        {
            // Delegate type
            var delegateType = typeof(ColumnValuesAssigner<,>).MakeGenericType(new Type[] { dataType, tupleType });

            // First create the function parameters
            var tuples = Expression.Parameter(tupleType.MakeArrayType(), "tuples");
            var data = Expression.Parameter(dataType, "data");
            var chunkSize = Expression.Parameter(typeof(int), "chunkSize");

            // Creating the (generated) method body.
            
            // Declaring variables which is in the body of (generated) method
            var index = Expression.Variable(typeof(int), "index");

            // Creating label for the loop (for the fulfilled exit condition because there is only 'while(true)' loop in the expression tree)
            LabelTarget label = Expression.Label(); // Creating a label to jump to from a loop.

            var currentColumnChunkField = dataType.GetFields()[columnIndex];

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
                        // columnsChunk.'current column fieldname'[index] =
                        Expression.Assign(Expression.ArrayAccess(Expression.Field(data, currentColumnChunkField), index),
                                            // tuples[index].'tuple fieldname related to current column'
                                            Expression.Field(Expression.ArrayIndex(tuples, index), currentColumnChunkField.Name)), 
                        // index++;
                        Expression.PostIncrementAssign(index)),
                    // else
                    Expression.Break(label, index)
                ),
               label)
            );

            var lambda = Expression.Lambda(delegateType, body, new ParameterExpression[] { tuples, data, chunkSize });

            var fn = lambda.Compile();

            return fn;
        }
    }
}
