// -----------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The logging level
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// No log level was specified
        /// </summary>
        None = 0,

        /// <summary>
        /// The log entry contains error information
        /// </summary>
        Error = 1,

        /// <summary>
        /// The log entry contains warning information
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The log entry contains general information
        /// </summary>
        Info = 4,

        /// <summary>
        /// The log entry contains detailed information
        /// </summary>
        DetailedInfo = 8,

        /// <summary>
        /// The log entry contains debugging information
        /// </summary>
        Debug = 16
    }
}
