using futura.Util;
using Dapper;
using Microsoft.Data.Sqlite;

namespace futura.pod_dump;

public class PodcastManager
{

    const string SQL_FIND_PODCASTS = "sql.find_podcasts.sql";
    const string SQL_GET_PODCAST_BY_ID = "sql.get_podcast.sql";

    public PodcastManager()
    {
        SetupSqlMapper();

    }

    /// <summary>
    /// Registers custom type mapper for Dapper -> Sqlite use.
    /// </summary>
    void SetupSqlMapper()
    {
        // Register type converter for dapper use
        //https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/dapper-limitations
        SqlMapper.AddTypeHandler(new GuidHandler());
    }

    public async Task<IEnumerable<dynamic>> FindPodcast(string searchTerm, bool exact)
    {
        var cfg = new ConfigManager();
        var cs = $"Data Source={cfg.ApplePodcastSqlitePath};Cache=Default;Mode=ReadOnly";

        //System.Diagnostics.Debug.WriteLine("cs: {0}", cs);

        using (var conn = new SqliteConnection(cs))
        {
            conn.Open();

            var sql = EmbeddedHelper.GetString(SQL_FIND_PODCASTS);

            string term = string.Empty;

            if (!exact)
            {
                term = $"%{searchTerm}%";
            }
            else
            {
                term = searchTerm;
            }

            var searchOptions = new
            {
                Search = term
            };

            var result = await conn.QueryAsync<dynamic>(sql, searchOptions);

            //System.Diagnostics.Debug.WriteLine("FindPodcast row count: {0}", result.Count());

            /*foreach (var data in result)
            {
                Out.Line($"id: {data.Podcast}");
            }*/

            return result;

        }
    }

}