﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.CodeGen
{
    /// <summary>
    /// Delegate that can parse a string value and assign it to a field of a structure.
    /// </summary>
    /// <typeparam name="T">Type of the structure.</typeparam>
    /// <param name="data">Reference to the structure that will receive the parsed value.</param>
    /// <param name="value">String value to parse into a variable.</param>
    /// <remarks>
    /// This delegate is used for parsing strings into tuples.
    /// </remarks>
    public delegate void ColumnParserDelegate<T>(ref T data, string value)
        where T : struct;
}
