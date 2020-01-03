using HelloCoreCommons.CommandLine;
using HelloCoreCommons.Configuration;

using HelloCoreDal.DataAccessLayer;

using Serilog;

namespace HelloCoreDal {
    public class DbSetupProgram : AbstractCommandLineProgram<DbContextArgs> {
        public DbSetupProgram(string[] args) : base(args) {
        }

        public static void Main(string[] args) {
            var seedProgram = new DbSetupProgram(args);
            seedProgram.Run();
        }

        public override void Execute() {
            var Configuration = ConfigurationTk.ConfigureFromFile();

            using var context = new DemoDbContextFactory().CreateDbContext(Args, Configuration);

            if (Options.DeleteIfExists) {
                context.Database.EnsureDeleted();
                Log.Information("Deleted database");
            }
            context.Database.EnsureCreated();
            Log.Information("Created database");

            context.SeedDatabase(Options.MasterUserIdentity);
            Log.Information("Populated database");
        }
    }
}