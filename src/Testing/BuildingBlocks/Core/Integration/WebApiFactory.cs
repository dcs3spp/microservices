using Autofac.Extensions.DependencyInjection;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.IO;
using System.Reflection;
using System.Net.Http;
using Xunit.Abstractions;

using dcs3spp.Extensions.Logging.Testing;
using dcs3spp.Testing.Utilities.Net.Http.Handlers;


namespace dcs3spp.Testing.Integration
{
    public class WebApiTestFactory<TStartupClass> : WebApplicationFactory<TStartupClass>, IDisposable 
        where TStartupClass : class
    {
        // track if Dispose has been called
        bool disposed = false;

        /// <summary>
        /// Delegate used by <see cref="WebApiTestFactory{TStartupClass}.CreateTestServer(Microsoft.AspNetCore.Hosting.IWebHostBuilder)"/> to seed the database.
        /// If unset no action is taken with database and server instance returned from base class.
        /// </summary>
        public Action<TestServer> OnServerCreated { get; set; }

        /// <inheritdoc />
        public ITestOutputHelper Output { get; set; }
        

        /// <summary>
        /// Build the web host to use XUnit Logging with TSartupClass
        /// </summary>
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup<TStartupClass>()
                .ConfigureLogging(logging => { 
                        logging.ClearProviders(); 
                        logging.AddXunit(Output);
                    });
        }

        /// <summary>
        /// Configure the webhost from appsettings.json from the path where the assembly is located for 
        /// the startup class. The Autofact container is configured here also.
        /// </summary>
        /// <seealso cref="Microsoft.AspNetCore.Mvc.Testing.ConfigureWebHost(IWebHostBuilder)"/></seealso>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var path = Assembly.GetAssembly(typeof(WebApiTestFactory<TStartupClass>)).Location;

            builder.UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                })
                .ConfigureServices(services => services.AddAutofac());

            base.ConfigureWebHost(builder);
        }

        /// <summary>
        /// Create a <see cref="Microsoft.AspNetCore.TestHost.TestServer"/> instance and applies
        /// <see cref="OnServerCreated"/> action for seeding database.
        /// </summary>
        /// <returns>Return <see cref="Microsoft.AspNetCore.TestHost.TestServer"/> instance. </returns>
        /// <seealso cref="Microsoft.AspNetCore.Mvc.Testing.CreateServer(IWebHostBuilder)"/></seealso>
        protected override TestServer CreateServer(IWebHostBuilder builder) 
        {
            TestServer server = base.CreateServer(builder);
            
            if(OnServerCreated != null)
            {
                OnServerCreated(server);
            }

            return server;
        }

        /// <summary>
        /// Create a <see cref="GrpcChannel"/> for a specified base address
        /// </summary>
        /// <param name="baseAddress">Client base address</param>
        /// <remarks>
        /// A handler is created on server to return a response that matches
        /// the HTTP version in the client request
        /// </remarks>
        /// <returns><see cref="Grpc.Net.Client.GrpcChannel"/> for the specified base address</returns>
        public GrpcChannel CreateChannel(string baseAddress)
        {
            Uri uri;
            bool valid = Uri.TryCreate(baseAddress, UriKind.Absolute, out uri);
            
            if(!valid) 
                throw new UriFormatException($"Invalid base address => {baseAddress}");

            var responseVersionHandler = new ResponseVersionHandler();
            responseVersionHandler.InnerHandler = this.Server.CreateHandler();

            HttpClient client = new HttpClient(responseVersionHandler);
            client.BaseAddress = new Uri(baseAddress);

            return GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }


        /// <summary>
        /// Update Xunit logging instance
        /// </summary>
        /// <param name="output">Xunit output stream</param>
        /// <returns>This instance</returns>
        public WebApiTestFactory<TStartupClass> SetOutput(ITestOutputHelper output)
        {
            Output = output;
            return this;
        }


        #region IDisposable
        /// <summary>
        /// Subclasses only provide Dispose(bool) and must call base.Dispose(disposing)
        /// </summary>
        /// <remarks>
        /// For further info see https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netcore-3.1
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return; 

            disposed = true;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~WebApiTestFactory()
        {
            Dispose(false);
        }
        #endregion
    }
}