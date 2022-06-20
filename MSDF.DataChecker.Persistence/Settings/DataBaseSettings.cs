namespace MSDF.DataChecker.Persistence.Settings
{
    public class DataBaseSettings
    {
        public string Engine { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string SqlServer { get; set; }
        public string PostgresSql { get; set; }
    }
}
