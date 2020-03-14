
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;

namespace dcs3spp.Testing.Data.Seed
{
    /// <summary>
    /// Abstract <c>DataSeedBase</c> class for setup and tear down of database
    /// <list type="bullet">
    /// <item>
    /// <term>UpAsync</term>
    /// <description>Asbtract seed database operation</description>
    /// </item>
    /// <item>
    /// <term>DownAsync</term>
    /// <description>Abstract deseed database operation</description>
    /// </item>
    /// <item>
    /// <term>SeedAsync</term>
    /// <description>Perform database seed operation, down followed by up</description>
    /// </item>
    /// </list>
    /// </summary>    
    public abstract class DataSeedBase : DataSeed
    {
        protected TestServer _server;

        /// <summary>
        /// Initialise test server
        /// </summary>
        public DataSeedBase(TestServer server)
        {
            _server = server;
        }

        /// <summary>
        /// Seed the database
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        public abstract Task<bool> UpAsync();

        /// <summary>
        /// Deseed the database
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        /// <remarks>Deseeding the database may correspond to removing data or removing the database</remarks>
        public abstract Task<bool> DownAsync();

        
        /// <summary>
        /// Perform the seed operation
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        /// <remarks>
        /// This performs DownAsync followed by UpAsync.
        /// Exceptions are logged in the down and up operations
        /// </remarks>
        public virtual async Task<bool> SeedAsync()
        {
            bool done = await DownAsync();
            if(done)
                return await UpAsync();
            else
                return false;
        }
    }
}