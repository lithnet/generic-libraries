// -----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Logging
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Microsoft.Win32;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides facilities for logging messages and exceptions to a file
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class Logger
    {
        /// <summary>
        /// A value indicating if multi-threaded synchronized logging is enabled
        /// </summary>
        [ThreadStatic]
        private static bool inThreadMode;

        /// <summary>
        /// The string builder for the thread if threaded logging is enabled
        /// </summary>
        [ThreadStatic]
        private static StringBuilder threadLogBuilder;

        [ThreadStatic]
        private static int padSpace = 0;

        /// <summary>
        /// The log writer object
        /// </summary>
        private static StreamWriter logWriter;
        
        /// <summary>
        /// Initializes static members of the Logger class
        /// </summary>
        static Logger()
        {
            Logger.LogLevel = LogLevel.Info;
            Logger.SyncObject = new object();
            Logger.SetDebugLevel();
        }

        /// <summary>
        /// The event log handler delegate
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event arguments</param>
        public delegate void EventLoggedHandler(object sender, EventLoggedArguments e);

        /// <summary>
        /// Occurs when an event has been logged
        /// </summary>
        public static event EventLoggedHandler OnEventLogged;

        /// <summary>
        /// Gets or sets an object used to synchronize writes to the log file
        /// </summary>
        public static object SyncObject { get; set; }

        /// <summary>
        /// Gets or sets the current log level
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        public static bool IncludeCallingMethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the log content should be written to the console
        /// </summary>
        public static bool OutputToConsole { get; set; }

        /// <summary>
        /// Gets or sets the log file path
        /// </summary>
        public static string LogPath { get; set; }
        
        /// <summary>
        /// Gets the underlying StreamWriter object
        /// </summary>
        private static StreamWriter LogWriter
        {
            get
            {
                if (Logger.logWriter == null)
                {
                    Logger.CreateLogWriter();
                }

                return Logger.logWriter;
            }
        }

        /// <summary>
        /// Closes the current log file
        /// </summary>
        public static void CloseLog()
        {
            try
            {
                if (Logger.logWriter != null)
                {
                    Logger.logWriter.Close();
                    Logger.logWriter = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clears the current log file
        /// </summary>
        public static void ClearLog()
        {
            if (Logger.inThreadMode)
            {
                Logger.EndThreadLog();
            }

            Logger.CloseLog();
            Logger.CreateLogWriter(true);
        }

        /// <summary>
        /// Gets the contents of the current log file
        /// </summary>
        /// <returns>A string containing the contents of the current log file</returns>
        public static string GetLogContent()
        {
            FileStream logFileStream = new FileStream(Logger.LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logReader = new StreamReader(logFileStream);
            string logContents = logReader.ReadToEnd();
            logReader.Close();
            logFileStream.Close();

            return logContents;
        }

        /// <summary>
        /// Writes the specified text to the log
        /// </summary>
        /// <param name="text">The text to write</param>

        public static void Write(string text, [CallerMemberName] string methodName = "Unknown")
        {
            Logger.WriteToLog(text, true, LogLevel.Info, false, null);
        }

        /// <summary>
        /// Writes the specified text to the log
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="logLevel">The level of this log message</param>
        public static void Write(string text, LogLevel logLevel)
        {
            Logger.WriteToLog(text, true, logLevel, false, null);
        }

        /// <summary>
        /// Writes the specified text to the log
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="parameters">The objects to replace in the formatted text string</param>
        public static void Write(string text, params object[] parameters)
        {
            Logger.WriteToLog(text, true, LogLevel.Info, false, parameters);
        }

        /// <summary>
        /// Writes the specified text to the log
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="logLevel">The level of this log message</param>
        /// <param name="parameters">The objects to replace in the formatted text string</param>
        public static void Write(string text, LogLevel logLevel, params object[] parameters)
        {
            Logger.WriteToLog(text, true, logLevel, false, parameters);
        }

        /// <summary>
        /// Writes the specified text to the log without timestamp or method names pre-pended to the text
        /// </summary>
        /// <param name="text">The text to write</param>
        public static void WriteRaw(string text)
        {
            Logger.WriteToLog(text, false, LogLevel.Info, false, null);
        }

        /// <summary>
        /// Writes the specified text to the log without timestamp or method names pre-pended to the text
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="parameters">The objects to replace in the formatted text string</param>
        public static void WriteRaw(string text, params object[] parameters)
        {
            Logger.WriteToLog(text, false, LogLevel.Info, false, parameters);
        }

        /// <summary>
        /// Writes the specified text to the log without timestamp or method names pre-pended to the text
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="logLevel">The level of this log message</param>
        public static void WriteRaw(string text, LogLevel logLevel)
        {
            Logger.WriteToLog(text, false, logLevel, false, null);
        }

        /// <summary>
        /// Writes the specified text to the log without timestamp or method names pre-pended to the text
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="logLevel">The level of this log message</param>
        /// <param name="parameters">The objects to replace in the formatted text string</param>
        public static void WriteRaw(string text, LogLevel logLevel, params object[] parameters)
        {
            Logger.WriteToLog(text, false, logLevel, false, parameters);
        }

        /// <summary>
        /// Writes the specified text to the log and appends a new line character
        /// </summary>
        /// <param name="text">The text to write</param>
        public static void WriteLine(string text)
        {
            Logger.WriteToLog(text, true, LogLevel.Info, true, null);
        }

        /// <summary>
        /// Writes the specified text to the log and appends a new line character
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="parameters">The objects to replace in the formatted text string</param>
        public static void WriteLine(string text, params object[] parameters)
        {
            Logger.WriteToLog(text, true, LogLevel.Info, true, parameters);
        }

        /// <summary>
        /// Writes the specified text to the log and appends a new line character
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="logLevel">The level of this log message</param>
        public static void WriteLine(string text, LogLevel logLevel)
        {
            Logger.WriteToLog(text, true, logLevel, true, null);
        }

        /// <summary>
        /// Writes the specified text to the log and appends a new line character
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="logLevel">The level of this log message</param>
        /// <param name="parameters">The objects to replace in the formatted text string</param>
        public static void WriteLine(string text, LogLevel logLevel, params object[] parameters)
        {
            Logger.WriteToLog(text, true, logLevel, true, parameters);
        }

        /// <summary>
        /// Writes a line of 79 of the specified separator character objects
        /// </summary>
        /// <param name="separatorChar">The character to use to create the separator line</param>
        public static void WriteSeparatorLine(char separatorChar)
        {
            Logger.WriteSeparatorLine(separatorChar, 79);
        }

        /// <summary>
        /// Writes a line of 79 of the specified separator character objects
        /// </summary>
        /// <param name="separatorChar">The character to use to create the separator line</param>
        public static void WriteSeparatorLine(char separatorChar, bool timeStamp)
        {
            Logger.WriteSeparatorLine(separatorChar, 79, timeStamp);
        }

        /// <summary>
        /// Writes a line of the specified number of separator characters
        /// </summary>
        /// <param name="separatorChar">The character to use to create the separator line</param>
        /// <param name="characterCount">The number of characters to use in creating the line</param>
        public static void WriteSeparatorLine(char separatorChar, int characterCount)
        {
            Logger.WriteSeparatorLine(separatorChar, 79, false);
        }

        /// <summary>
        /// Writes a line of the specified number of separator characters
        /// </summary>
        /// <param name="separatorChar">The character to use to create the separator line</param>
        /// <param name="characterCount">The number of characters to use in creating the line</param>
        public static void WriteSeparatorLine(char separatorChar, int characterCount, bool timeStamp)
        {
            string text = string.Empty;

            text = text.PadRight(characterCount, separatorChar);
            Logger.WriteToLog(text, timeStamp, LogLevel.Info, true, null);
        }

        /// <summary>
        /// Holds all writes to the log file on the current thread until <c ref="EndThreadLog"/> is called
        /// </summary>
        public static void StartThreadLog()
        {
            if (!Logger.inThreadMode)
            {
                Logger.threadLogBuilder = new StringBuilder();
                Logger.inThreadMode = true;
            }
        }

        /// <summary>
        /// Commits all writes from the current thread to the log file, and resumes normal logging operation
        /// </summary>
        public static void EndThreadLog()
        {
            try
            {
                if (Logger.inThreadMode)
                {
                    lock (Logger.SyncObject)
                    {
                        Logger.logWriter.Write(threadLogBuilder.ToString());
                    }
                }
            }
            finally
            {
                Logger.threadLogBuilder = null;
                Logger.inThreadMode = false;
            }
        }

        public static void IncreaseIndent()
        {
            padSpace++;
        }

        public static void DecreaseIndent()
        {
            if (padSpace > 0)
            {
                padSpace--;
            }
        }

        /// <summary>
        /// Gets the current steam for the log file
        /// </summary>
        /// <returns>A new FileStream object for the log file</returns>
        public static FileStream GetLogStream()
        {
            return new FileStream(Logger.LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// Writes an exception to the log
        /// </summary>
        /// <param name="ex">The exception to write</param>
        public static void WriteException(Exception ex)
        {
            Logger.WriteException(ex, LogLevel.Info);
        }

        /// <summary>
        /// Writes an exception to the log
        /// </summary>
        /// <param name="ex">The exception to write</param>
        /// <param name="logLevel">The level of this log message</param>
        public static void WriteException(Exception ex, LogLevel logLevel)
        {
            string messageText = GetExceptionText(ex, false);
            Logger.WriteToLog(messageText, false, logLevel, true);
        }

        /// <summary>
        /// Gets a text representation of the exception
        /// </summary>
        /// <param name="ex">The exception to process</param>
        /// <param name="isInnerException">A value indicating whether this is an inner exception</param>
        /// <returns>A string representing the content of the exception message</returns>
        public static string GetExceptionText(this Exception ex, bool isInnerException = false)
        {
            StringBuilder messageText = new StringBuilder();

            messageText.AppendLine(string.Empty.PadRight(40, '*'));

            if (isInnerException)
            {
                messageText.AppendLine("Inner exception details");
            }
            else
            {
                messageText.AppendFormat("An exception has occurred in {0}\n", GetCallingMethodName());
            }

            messageText.AppendFormat("Type: {0}\n", ex.GetType().ToString());
            messageText.AppendLine(GetExceptionSpecificDetails(ex));
            messageText.AppendFormat("Message: {0}\n", ex.Message);
            messageText.AppendFormat("Source: {0}\n", ex.Source);

            if (ex.TargetSite != null)
            {
                messageText.AppendFormat("TargetSite: {0}\n", ex.TargetSite.ToString());
            }

            messageText.AppendLine("StackTrace:");
            messageText.AppendLine(ex.StackTrace);
            messageText.AppendLine(string.Empty.PadRight(40, '*'));

            if (ex.InnerException != null)
            {
                messageText.Append(GetExceptionText(ex.InnerException, true));
            }

            return messageText.ToString();
        }

        /// <summary>
        /// Gets type-specific exception details
        /// </summary>
        /// <param name="ex">The exception to process</param>
        /// <returns>A string representing the type-specific content of the exception message</returns>
        private static string GetExceptionSpecificDetails(Exception ex)
        {
            StringBuilder messageText = new StringBuilder();

            if (ex is Win32Exception)
            {
                Win32Exception w32ex = ex as Win32Exception;
                messageText.AppendLine(string.Format("Win32 Error Code: {0}", w32ex.ErrorCode));
                messageText.AppendLine(string.Format("Win32 Native Error Code: {0}", w32ex.NativeErrorCode));
            }
            else if (ex is COMException)
            {
                messageText.AppendLine(string.Format("COM Error Code: {0}", ((COMException)ex).ErrorCode.ToString()));
            }
            else if (ex is ManagementException)
            {
                messageText.AppendLine("WMI Error: " + ((ManagementException)ex).ErrorCode.ToString());
            }
            else if (ex is System.Net.WebException)
            {
                messageText.AppendLine("Status: " + ((System.Net.WebException)ex).Status.ToString());
            }

            return messageText.ToString();
        }

        /// <summary>
        /// Creates a new log writer
        /// </summary>
        private static void CreateLogWriter()
        {
            Logger.CreateLogWriter(false);
        }

        /// <summary>
        /// Creates a new log writer
        /// </summary>
        /// <param name="truncateLog">A value indicating whether the existing log file should be truncated</param>
        private static void CreateLogWriter(bool truncateLog)
        {
            FileMode filemode = truncateLog ? FileMode.Create : FileMode.Append;
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name + ".log";

            // Create the file using the specified path if it exists
            if (!string.IsNullOrWhiteSpace(Logger.LogPath))
            {
                Logger.InitializeStreamWriter(Logger.LogPath, filemode);
                return;
            }
            else
            {
                // Fall back to the path specified on the command line if it exists
                Logger.LogPath = Environment.ExpandEnvironmentVariables(GetLogPathFromCommandLine());
            }

            if (!string.IsNullOrWhiteSpace(Logger.LogPath))
            {
                Logger.InitializeStreamWriter(Logger.LogPath, filemode);
                return;
            }

            // Try and create a log file in the application folder
            Logger.LogPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), assemblyName);
            if (Logger.TryInitializeStreamWriter(Logger.LogPath, filemode))
            {
                return;
            }

            // Try and create a log file in the user's temp folder
            Logger.LogPath = Path.Combine(System.IO.Path.GetTempPath(), assemblyName);
            if (Logger.TryInitializeStreamWriter(Logger.LogPath, filemode))
            {
                return;
            }
            else
            {
                throw new FileNotFoundException("The specified log file could not be found or opened");
            }
        }
        
        /// <summary>
        /// Attempts to initialize the stream writer for the specified file
        /// </summary>
        /// <param name="fileName">The file name to open</param>
        /// <param name="filemode">The file access mode</param>
        /// <returns>A value indicating if the stream writer was successfully create</returns>
        private static bool TryInitializeStreamWriter(string fileName, FileMode filemode)
        {
            try
            {
                Logger.InitializeStreamWriter(fileName, filemode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes the stream writer for the specified file
        /// </summary>
        /// <param name="fileName">The file name to open</param>
        /// <param name="filemode">The file access mode</param>
        private static void InitializeStreamWriter(string fileName, FileMode filemode)
        {
            string logpath = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(logpath);
            }

            Logger.logWriter = CreateStreamWriter(fileName, filemode, true);
        }

        /// <summary>
        /// Writes the specified log message to the log file
        /// </summary>
        /// <param name="text">The formatted text to log</param>
        /// <param name="timeStamp">A value indicating whether the text should be pre-pended with a timestamp</param>
        /// <param name="addProcedureName">A value indicating whether the text should be pre-pended with the calling method name</param>
        /// <param name="logLevel">The log level of the message</param>
        /// <param name="newLine">A value indicating whether a new line character should be appended to the message</param>
        /// <param name="parameters">A list of objects to replace in the formatted string</param>
        private static void WriteToLog(string text, bool timeStamp, LogLevel logLevel, bool newLine, params object[] parameters)
        {
            EventLoggedArguments args = new EventLoggedArguments();

            args.Level = logLevel;
            if (parameters != null && parameters.Count() > 0)
            {
                text = string.Format(text, parameters);
            }

            lock (Logger.SyncObject)
            {
                {
                    Debug.WriteLine(text);

                    if (Logger.LogLevel >= logLevel)
                    {
                        if (Logger.OutputToConsole)
                        {
                            Console.Write(string.Empty.PadLeft(padSpace * 4) + text + (newLine ? "\n" : string.Empty));
                        }

                        if (Logger.IncludeCallingMethodName)
                        {
                            args.CallingMethod = GetCallingMethodName();
                            text = string.Format("{0}: {1}", args.CallingMethod, text);
                        }

                        text = string.Empty.PadLeft(padSpace * 4, ' ') + text;


                        if (timeStamp)
                        {
                            args.TimeStamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                            text = string.Format("{0}: {1}", args.TimeStamp, text);
                        }

                        if (padSpace > 0)
                        {
                            text = text.Replace("\n", "\n" + string.Empty.PadLeft((padSpace * 4) + (args.TimeStamp == null ? 0 : args.TimeStamp.Length), ' '));
                        }

                        if (newLine)
                        {
                            text = text + "\n";
                        }

                        if (Logger.inThreadMode)
                        {
                            Logger.threadLogBuilder.Append(text);
                        }
                        else
                        {
                            Logger.LogWriter.Write(text);
                        }

                        args.Message = text;

                        if (Logger.OnEventLogged != null)
                        {
                            Logger.OnEventLogged(null, args);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new StreamWriter object from the specified file
        /// </summary>
        /// <param name="fileName">The file name to open</param>
        /// <param name="fileMode">The file access mode</param>
        /// <param name="retryOnError">A value indicating whether the operation should be retried if it fails</param>
        /// <returns>A StreamWriter for the specified file</returns>
        private static StreamWriter CreateStreamWriter(string fileName, FileMode fileMode, bool retryOnError = false)
        {
            try
            {
                FileStream logFileStream = new FileStream(fileName, fileMode, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter logStreamWriter = new StreamWriter(logFileStream);
                logStreamWriter.AutoFlush = true;
                return logStreamWriter;
            }
            catch (Exception)
            {
                if (retryOnError && File.Exists(fileName))
                {
                    Thread.Sleep(5000);
                    return CreateStreamWriter(fileName, fileMode, false);
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the log file path that was specified on the command line
        /// </summary>
        /// <returns>The log file path if one was specified on the command line, otherwise an empty string is returned</returns>
        private static string GetLogPathFromCommandLine()
        {
            string fileName = string.Empty;

            string argument = Environment.GetCommandLineArgs().FirstOrDefault(t =>
                t.StartsWith("/logfile:") ||
                t.StartsWith("/log:"));

            if (!string.IsNullOrWhiteSpace(argument))
            {
                fileName = argument.Remove(0, argument.IndexOf(":") + 1);
            }

            return fileName.Trim('"');
        }

        /// <summary>
        /// Evaluates if a debugger is attached, or if the /debug switch was specified on the command line, and if so, sets the LogLevel to Debug
        /// </summary>
        private static void SetDebugLevel()
        {
            string executable;

            if (Debugger.IsAttached ||
                Environment.GetCommandLineArgs().Any(t => t.Equals("/debug", StringComparison.CurrentCultureIgnoreCase)))
            {
                Logger.LogLevel = LogLevel.Debug;
                return;
            }

            try
            {
                executable = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }
            catch
            {
                return;
            }

            if (executable == null)
            {
                return;
            }

            try
            {
                RegistryKey hkcuKey = Registry.CurrentUser.OpenSubKey(@"Software\Lithnet\Logging\" + executable, false);
                if (hkcuKey != null)
                {
                    object loglevel = hkcuKey.GetValue("LogLevel", null);
                    if (loglevel != null && loglevel is int)
                    {
                        Logger.LogLevel = (LogLevel)loglevel;
                    }
                }
            }
            catch
            {
            }

            try
            {
                RegistryKey hklmKey = Registry.LocalMachine.OpenSubKey(@"Software\Lithnet\Logging\" + executable, false);
                if (hklmKey != null)
                {
                    object loglevel = hklmKey.GetValue("LogLevel", null);
                    if (loglevel != null && loglevel is int)
                    {
                        Logger.LogLevel = (LogLevel)loglevel;
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the name of the method that called the log function
        /// </summary>
        /// <returns>The name of the method that called the log function</returns>
        private static string GetCallingMethodName()
        {
            try
            {
                StackTrace stackTrace = new StackTrace();

                foreach (StackFrame stackFrame in stackTrace.GetFrames())
                {
                    if (stackFrame.GetMethod().DeclaringType.FullName != stackTrace.GetFrame(0).GetMethod().DeclaringType.FullName)
                    {
                        return stackFrame.GetMethod().Name;
                    }
                }
            }
            catch
            {
            }

            return "Unknown calling method";
        }
    }
}
