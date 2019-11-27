using System;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;

namespace SignalRServer
{
    public class MainHub : Hub
    {

        private bool IsRunning = false;

        public Task Teste(string unidade)
        {
            Console.WriteLine(Program.GetTime() + "[PrintHub.HandShake] " + unidade + " Informou que está aberta a solicitações");

            Program.CallerController.Callers.Add(new TesteCaller(unidade, Clients.Caller, true));

            Console.WriteLine(Program.GetTime() + "[PrintHub.HandShake] Caller: " + unidade + " adicionado");

            if (!IsRunning)
                Parallel.Invoke(Run);

            return Clients.Caller.SendAsync("HandShaked");
        }
        
        public void Run()
        {

            IsRunning = true;

            Console.WriteLine(Program.GetTime() + "[PrintHub.Run] Executando...");
            while (IsRunning)
            {
                var lstPendencia = Program.lstPendencias.Where(p => p.status == Program.Pendencia.StatusPendencia.Open).ToList();
                if (lstPendencia.Count <= 0)
                    continue;

                foreach (var Caller in Program.CallerController.Callers.ToList())
                {
                    foreach (var pendencia in lstPendencia.Where(p => p.unidade == Caller.Unidade))
                    {
                        Caller.Execute(pendencia.etiqueta)
                            .ContinueWith((task) =>
                            {
                                Console.WriteLine(task.Status);
                                Program.lstPendencias.Remove(pendencia);
                                Console.WriteLine("Removeu pendencia: " + pendencia.unidade + "/" + pendencia.etiqueta);
                            })
                            .Wait();

                        pendencia.status = Program.Pendencia.StatusPendencia.Processing;
                    }
                }
            }

            Console.WriteLine(Program.GetTime() + "[PrintHub.Run] Finalizado");
        }

    }
}