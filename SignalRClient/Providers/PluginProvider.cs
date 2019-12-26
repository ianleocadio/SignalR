using Microsoft.Extensions.Logging;
using SignalRLibrary;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace SignalRClient
{
    public class PluginProvider
    {

        private readonly IServiceProvider _serviceProvider;

        public PluginProvider(IServiceProvider serviceProvider)
        {
            
            _serviceProvider = serviceProvider;
            Compose();
        }

        [ImportMany]
        public IEnumerable<Plugin> Plugins { get; private set; }

        private void Compose()
        {
            // Catalogs does not exists in Dotnet Core, so you need to manage your own.
            var assemblies = new List<Assembly>() { typeof(Program).GetTypeInfo().Assembly };
            var pluginAssemblies = Directory.GetFiles("c:\\plugins\\", "*.dll", SearchOption.TopDirectoryOnly)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                // Ensure that the assembly contains an implementation for the given type.
                .Where(s => s.GetTypes().Where(p => typeof(Plugin).IsAssignableFrom(p)).Any());

            assemblies.AddRange(pluginAssemblies);

            var configuration = new ContainerConfiguration()
                .WithAssemblies(assemblies);

            using (var container = configuration.CreateContainer())
            {
                Plugins = container.GetExports<Plugin>().Select(p =>
                {
                    Type loggerType = typeof(ILogger<>);
                    Type loggerContructedType = loggerType.MakeGenericType(p.GetType());
                    p.Logger = (ILogger)_serviceProvider.GetService(loggerContructedType);
                    return p;
                });
            }
        }
    }
}
