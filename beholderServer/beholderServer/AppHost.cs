using Funq;
using ServiceStack;
using ServiceStack.Formats;
using ServiceStack.Logging;
using ServiceStack.Configuration;

using beholderServer.ServiceInterface;



namespace beholderServer
{
    
    public class AppHost : AppHostBase
    {

        /// <summary>c
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("beholderServer", typeof(MyServices).Assembly)
        {
            
        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());
            container.Register<IAppSettings>(new AppSettings());

        }
    }
}