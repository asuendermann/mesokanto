#region

using System;
using System.Diagnostics;
using System.Linq;

using CommandLine;

using HelloCoreCommons.Configuration;

using Serilog;

#endregion

namespace HelloCoreCommons.CommandLine {
    public abstract class AbstractCommandLineProgram<T>
        where T : BaseCommandLineOptions, new() {
        protected readonly string[] Args;

        protected AbstractCommandLineProgram(string[] args) {
            this.Args = args;
            ConfigurationTk.ConfigureSerilogFromFile();
            Options = ParseArguments(args);
            CheckForPriorProcess();
        }

        public T Options { get; }

        /// <summary>
        ///     Property indicating whether another process with same name is already running.
        /// </summary>
        /// <returns>
        ///     Returns true if ther is already a running process with the same name as the current one,
        ///     or false if the current process is unique.
        /// </returns>
        private static void CheckForPriorProcess() {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            var priorProcess = processes.Any
            (p => p.Id != currentProcess.Id &&
                  p.MainModule?.FileName == currentProcess.MainModule?.FileName);
            if (priorProcess) {
                throw new ArgumentException(CommandLineResources.Error_NotParsed);
            }
        }

        public static T ParseArguments(string[] args) {
            if (null != args) {
                var result = Parser.Default.ParseArguments<T>(args);
                if (result.Tag == ParserResultType.Parsed) {
                    var parsed = result as Parsed<T>;
                    var options = parsed?.Value;
                    if (null != options) {
                        return options;
                    }
                }
            }

            throw new ArgumentException(CommandLineResources.Error_NotParsed);
        }

        public abstract void Execute();

        public void Run() {
            try {
                Execute();
            } catch (Exception exception) {
                Log.Error(exception, CommandLineResources.Error_UnexpectedTermination);
            } finally {
                if (Options.Hold) {
                    Console.WriteLine();
                    Console.WriteLine(CommandLineResources.Message_Final);
                    Console.ReadLine();
                }
            }

            Environment.Exit(0);
        }
    }
}