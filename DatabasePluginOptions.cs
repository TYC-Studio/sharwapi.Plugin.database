namespace Tyc_studio.Plugin.Database
{
    public class DatabasePluginOptions
    {
        public const string SectionName = "DatabasePlugin";
        public bool ExposePluginWebApi { get; set; } = false;
    }

    public interface IDatabaseContext
    {
        public string ConnectionString { get; set; }
        public string DatabaseType { get; set; }
        public int CommandTimeout { get; set; }
        public bool UseTransaction { get; set; }
    }
    public class DatabaseContext : IDatabaseContext
    {
        public const string SectionName = "DatabaseContext";
        public required string ConnectionString { get; set; }
        public string DatabaseType { get; set; } = "SqlServer";
        public int CommandTimeout { get; set; } = 10;
        public bool UseTransaction { get; set; } = true;
    }
}