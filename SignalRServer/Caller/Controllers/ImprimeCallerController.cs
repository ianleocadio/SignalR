using Microsoft.Extensions.Logging;
using SignalRClient.Logging;
using SignalRServer.Caller.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRServer.Caller.Controllers
{
    public class ImprimeCallerController : CallerController<ImprimeCaller>
    {
        private CancellationTokenSource _cancellationTokenSource;   
        
        private Task _task;
        public Task TaskInstance
        {
            get
            {
                if (_task == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _task = new Task(Run, _cancellationTokenSource.Token);
                }

                return _task;
            }
        }

        public ImprimeCallerController(ILogger<ImprimeCallerController> logger) : base(logger)
        {
        }

        public void RunInstance()
        {
            if (TaskInstance.Status == TaskStatus.Running)
                return;

            if (TaskInstance.Status != TaskStatus.Created)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _task = null;
            }

            TaskInstance.Start();
        }


        public void Run()
        {
            _logger.LogInformation("Executando...");

            // Pode ser controlado por fora atráves de um CancellationToken
            while (true)
            {
                try
                {
                    var lstPendencia = Program.lstPendencias
                            .Where(p => Program.Pendencia.StatusPendencia.Open == p?.status)
                            .ToList();
                    if (lstPendencia == null || lstPendencia.Count <= 0)
                        continue;

                    foreach (var Caller in Callers.Values.ToList())
                    {
                        foreach (var pendencia in lstPendencia.Where(p => p?.unidade == Caller.Unidade))
                        {
                            _logger.LogInformation("Executando {Event} pela Unidade: {Unidade} com a etiqueta: {Etiqueta}", Caller.Event, Caller.Unidade, pendencia.etiqueta);
                            Caller.Execute(pendencia.etiqueta)
                                .ContinueWith((task) =>
                                {
                                    Program.lstPendencias.Remove(pendencia);
                                    _logger.LogInformation("Removeu pendência: {unidade}/{etiqueta}", pendencia.unidade, pendencia.etiqueta);
                                });

                            pendencia.status = Program.Pendencia.StatusPendencia.Processing;
                        }
                    }
                }
                catch (IndexOutOfRangeException) { }
                catch (NullReferenceException) { }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                }
            }

        }
    }
}
