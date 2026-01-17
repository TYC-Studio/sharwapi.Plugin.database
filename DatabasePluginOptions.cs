namespace sharwapi.Plugin.database
{
    public class DatabasePluginOptions
    {
        public const string SectionName = "DatabasePlugin";
        public bool ExposePluginWebApi { get; set; } = false;
    }

    public class DatabaseContext
    {
        public required string ConnectionString { get; set; }
        public string DatabaseType { get; set; } = "SqlServer";
        public int CommandTimeout { get; set; } = 10;
        public bool UseTransaction { get; set; } = true;
    }
}