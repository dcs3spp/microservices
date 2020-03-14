using System.Threading.Tasks;

namespace dcs3spp.Testing.Data.Seed
{
    /// <summary>
    /// <c>DataSeed</c> interface for setup and tear down of database
    /// <list type="bullet">
    /// <item>
    /// <term>UpAsync</term>
    /// <description>Seed database operation</description>
    /// </item>
    /// <item>
    /// <term>DownAsync</term>
    /// <description>Deseed database operation</description>
    /// </item>
    /// </list>
    /// </summary>
    public interface DataSeed
    {
        /// <summary>
        /// Seed the database
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        Task<bool> UpAsync();

        /// <summary>
        /// Deseed the database
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        Task<bool> DownAsync();

        /// <summary>
        /// Perform data seed operation
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        Task<bool> SeedAsync();
    }
}