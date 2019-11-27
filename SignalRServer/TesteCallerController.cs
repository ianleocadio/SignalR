using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class TesteCallerController : CallerController<TesteCaller>
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
            
            Console.WriteLine(Program.GetTime() + "[TesteCallerController.Run] Executando...");
            while (true)
            {
                try
                {
                    var lstPendencia = Program.lstPendencias.Where(p => p.status == Program.Pendencia.StatusPendencia.Open).ToList();
                    if (lstPendencia.Count <= 0)
                        continue;

                    foreach (var Caller in base.Callers.ToList())
                    {
                        foreach (var pendencia in lstPendencia.Where(p => p.unidade == Caller.Unidade))
                        {
                            //Console.WriteLine("[Loop lstPendencia] - " + pendencia.etiqueta + " " + pendencia.status);
                            Caller.Execute(pendencia.etiqueta)
                                .ContinueWith((task) =>
                                {
                                    Program.lstPendencias.Remove(pendencia);
                                    Console.WriteLine("Removeu pendencia: " + pendencia.unidade + "/" + pendencia.etiqueta);
                                });

                            pendencia.status = Program.Pendencia.StatusPendencia.Processing;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }

            Console.WriteLine(Program.GetTime() + "[TesteCallerController.Run] Finalizado");
        }
    }
}
