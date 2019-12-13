using SignalRLibrary;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace SignalRPrototype.Drivers.drvPluginEtiquetadoraImpressao
{
    [Export(typeof(IPlugin))]
    public class EtiquetadoraImpressao : IPlugin
    {

        [EndPoint("Imprime")]
        public Task ImprimeEtiqueta(string etiqueta)
        {
            Console.WriteLine($"Imprime etiqueta: {etiqueta}");
            return Task.Delay(5000);
        }
    }
}
