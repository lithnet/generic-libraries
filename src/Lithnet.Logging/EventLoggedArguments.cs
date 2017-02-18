// -----------------------------------------------------------------------
// <copyright file="EventLoggedArguments.cs" company="Lithnet">
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
    /// Arguments passed when an EventLogged event is raised
    /// </summary>
    public class EventLoggedArguments
    {
        /// <summary>
        /// Gets or sets the message that was logged
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the message was logged
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the method that called the logger
        /// </summary>
        public string CallingMethod { get; set; }

        /// <summary>
        /// Gets or sets the log level of the message
        /// </summary>
        public LogLevel Level { get; set; }
    }
}
