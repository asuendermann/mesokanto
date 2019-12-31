using System.Collections.Generic;
using System.Data;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace HelloCore.SerilogExtensions {
    public static class SerilogTk {
        public const string Table_Serilog = "SeriLog";

        /// <summary>
        ///     create a standard Serilog MSSqlServer configuration.
        ///     application.json should set minimum level and provide a connection string name in the app settings.
        /// </summary>
        /// <param name="configuration"></param>
        public static void ConfigureMsSqlServer(IConfigurationRoot configuration) {
            var connectionString = configuration.GetConnectionString("LocalMesokantoDb");
/*
            var columnOptions = new ColumnOptions {
                //AdditionalColumns = new List<SqlColumn> {
                //    new SqlColumn {
                //        ColumnName = "MachineName",
                //        DataType = SqlDbType.NVarChar,
                //        DataLength = 64
                //    },
                //    new SqlColumn {
                //        ColumnName = "ThreadId",
                //        DataType = SqlDbType.Int
                //    },
                //    new SqlColumn {
                //        ColumnName = "ProjectName",
                //        DataType = SqlDbType.NVarChar,
                //        DataLength = 128
                //    }
                //}
            };
*/
            //columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            //columnOptions.Store.Remove(StandardColumn.Properties);
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString, Table_Serilog, configuration, autoCreateSqlTable: true)
                //.Enrich.FromLogContext()
                //.Enrich.WithMachineName()
                //.Enrich.WithThreadId()
                //.Enrich.With(new ProjectNameEnricher(configuration))
                //.ReadFrom.Configuration(configuration)
                ;

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}