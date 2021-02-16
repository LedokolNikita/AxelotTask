using System;
using System.Threading;
using System.Text;

namespace AxelotTask
{
    public class ReportBuilder
    {
        Random rand = new Random(DateTime.Now.Millisecond);

        public byte[] Build(CancellationToken cancellationToken)
        {
            try
            {
                return EmulationReportBuild(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return Encoding.UTF8.GetBytes($"Report stoped");
            }
        }

        private byte[] EmulationReportBuild(CancellationToken cancellationToken)
        {
            var timeInSeconds = StartWaiting(cancellationToken);
            GetProbabilisticError();
            return Encoding.UTF8.GetBytes($"Report ready at [{timeInSeconds}] s.");
        }

        private int StartWaiting(CancellationToken cancellationToken)
        {
            var timeInSeconds = GetRandomTime();
            for (int i = 0; i < timeInSeconds; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(1000); 
            }
            return timeInSeconds;
        }

        private int GetRandomTime()
        {
            return rand.Next(5, 45);
        }

        private void GetProbabilisticError()
        {
            if (rand.Next(0, 100) < 20)
            {
                throw new ReportFailedException("Report failed");
            }
        }
    }
}
