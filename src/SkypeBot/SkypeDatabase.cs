using System.Data.SQLite;

namespace SkypeBot
{
    public class SkypeDatabase
    {
        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public string ConnectionString { get; private set; }

        public SkypeDatabase(string path)
        {
            this.Path = path;
            this.DisplayName = GetDisplayName(path);
            this.ConnectionString = GetConnectionString(path);
        }

        public SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection();
            connection.ConnectionString = this.ConnectionString;
            connection.Open();

            return connection;
        }

        private static string GetDisplayName(string path)
        {
            return path;
        }

        private static string GetConnectionString(string path)
        {
            var csb = new SQLiteConnectionStringBuilder();
            csb.DataSource = path;
            csb.Version = 3;

            return csb.ConnectionString;
        }
    }
}
