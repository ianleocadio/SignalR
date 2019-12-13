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
        public PluginProvider()
        {
            Compose();
        }

        [ImportMany]
        public IEnumerable<IPlugin> Plugins { get; private set; }

        private void Compose()
        {
            // Catalogs does not exists in Dotnet Core, so you need to manage your own.
            var assemblies = new List<Assembly>() { typeof(Program).GetTypeInfo().Assembly };
            var pluginAssemblies = Directory.GetFiles("c:\\plugins\\", "*.dll", SearchOption.TopDirectoryOnly)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                // Ensure that the assembly contains an implementation for the given type.
                .Where(s => s.GetTypes().Where(p => typeof(IPlugin).IsAssignableFrom(p)).Any());

            assemblies.AddRange(pluginAssemblies);

            var configuration = new ContainerConfiguration()
                .WithAssemblies(assemblies);

            using (var container = configuration.CreateContainer())
            {
                Plugins = container.GetExports<IPlugin>();
            }
        }
    }
}
