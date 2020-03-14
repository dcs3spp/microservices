using System;
using Xunit.Abstractions;


namespace dcs3spp.Testing.Integration
{
    /// <summary>
    /// Helper class for use in integration tests.
    /// Encapsulates a WebApiTestFactory instance for creating TestServer, HttpClient and Grpc Channels
    /// </summary>
    public sealed class TestingBase<TStartup> : IDisposable where TStartup : class
    {
        #region Members
        // Factory for creating TestServer, GrpcChannel and HttpClient
        private WebApiTestFactory<TStartup> factory;
        
        // Track whether Dispose has been called.
        bool disposed = false;
        #endregion

        #region Properties
        /// <summary>
        /// Accessor for WebApiTestFactory that provides access to test server, HttpClient's etc.
        /// </summary>
        public  WebApiTestFactory<TStartup> Factory
        {
            get 
            {
                return factory;
            }
            private set
            {
                factory = value;
            }
        }

        /// <summary>
        /// Xunit output stream
        /// </summary>
        public ITestOutputHelper Output 
        { 
            get
            {
                return factory.Output;
            }  
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Create a WebApiTestFactory that creates a TestServer bootstrapped with TStartup class
        /// </summary>
        /// <param name="output">Xunit output stream</param>
        /// <remarks>Xunit logging is configured on TestServer</remarks>
        public TestingBase(ITestOutputHelper output)
        {
            factory = new WebApiTestFactory<TStartup>();
            factory = factory.SetOutput(output);
            Factory = factory;
        }
        #endregion


        #region IDisposable
        /// <summary>
        /// Dispose the WebApplicationFactory
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    factory.Dispose();
                }

                // unmanaged resources clean if any

                // notify disposing is complete
                disposed = true;
            }
        }
        #endregion
    }
}
