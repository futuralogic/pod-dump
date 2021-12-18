using futura.Util;
using Dapper;
using Microsoft.Data.Sqlite;

namespace futura.pod_dump;

public class PodcastManager
{

    /// <summary>
    /// Searches podcasts by podcast title.
    /// </summary>
    const string SQL_FIND_PODCASTS = "sql.find_podcasts.sql";

    /// <summary>
    /// Returns a list of episodes for a list of podcast ID's (ZUUID list). Pass a single ZUUID for a single padcoast. (That are stored in new storage location)
    /// </summary>
    const string SQL_GET_EPISODES_BY_PODCAST_ID = "sql.get_episodes_by_podcast_id_list.sql";

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

            // Debug info if needed:
            //System.Diagnostics.Debug.WriteLine("FindPodcast row count: {0}", result.Count());

            /*foreach (var data in result)
            {
                Out.Line($"id: {data.Podcast}");
            }*/

            return result;

        }
    }

    /// <summary>
    /// Enrich a collection of registrations with podcast data from Apple's database.
    /// </summary>
    /// <param name="registrations"></param>
    public async Task EnrichRegistrations(IEnumerable<Registration> registrations)
    {
        // Load all registrations
        var subscribedUUIDs = registrations.Select(r => Guid.Parse(r.Id));
        var podcastData = await GetEpisodeData(subscribedUUIDs);

        //await LoadSqlPodcastData(registrations);
        foreach (var reg in registrations)
        {
            EnrichRegistration(reg, podcastData.Where(p => p.Id.Equals(reg.Id)));
        }
    }

    /// <summary>
    /// Enrich a single registration with poadcast data from Apple's database.
    /// </summary>
    /// <param name="registration">Registration to enrich with metadata.</param>
    /// <param name="podcastData">Collection of episodes for the given podcast.</param>
    public void EnrichRegistration(Registration registration, IEnumerable<dynamic> podcastData)
    {
        var episodes = from podcast in podcastData
                       where
                           podcast.Id == registration.Id
                       select podcast;

        registration.PendingEpisodes = episodes.Count();
        registration.LastProcessedText = null;
    }

    /// <summary>
    /// Retrieve episode data for single podcast.
    /// </summary>
    /// <param name="podcastUUIds"></param>
    /// <returns></returns>
    public Task<IEnumerable<dynamic>> GetEpisodeData(Guid podcastUUId)
    {
        var regList = new List<Guid> { podcastUUId };
        return GetEpisodeData(regList);
    }

    /// <summary>
    /// Retrieve episode data for a collection of Apple podcast ZUUID (unique id's) from user's Sqlite database.
    /// </summary>
    /// <param name="podcastUUIds"></param>
    /// <returns></returns>
    async Task<IEnumerable<dynamic>> GetEpisodeData(IEnumerable<Guid> podcastUUIds)
    {
        var cfg = new ConfigManager();
        var cs = $"Data Source={cfg.ApplePodcastSqlitePath};Cache=Default;Mode=ReadOnly";

        //System.Diagnostics.Debug.WriteLine("cs: {0}", cs);

        using (var conn = new SqliteConnection(cs))
        {
            conn.Open();

            var sql = EmbeddedHelper.GetString(SQL_GET_EPISODES_BY_PODCAST_ID);

            string term = string.Empty;

            var queryParams = new
            {
                Ids = podcastUUIds
            };

            var result = await conn.QueryAsync<dynamic>(sql, queryParams);

            // Debug info if needed:
            System.Diagnostics.Debug.WriteLine("row count: {0}", result.Count());

            foreach (var data in result)
            {
                Out.Line($"id:  {data.Episode} - {data.Url}");
            }

            return result;

        }
    }


}