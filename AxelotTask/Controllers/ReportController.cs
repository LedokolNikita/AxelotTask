using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AxelotTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private static int idRequestReport = 0;
        private static Dictionary<int, CancellationTokenSource> dictionaryOfIdAndCancelToken =
            new Dictionary<int, CancellationTokenSource>();
        private object locker = new object();
        ReportBuilder reportBuilder = new ReportBuilder();
        Reporter reporter = new Reporter();

        [HttpGet("build")]
        public async Task<int> Build()
        {
            Stopwatch stopwatch = new Stopwatch();
            var currentId = 0;
            lock (locker)
            {
                currentId = idRequestReport++;
            }

            dictionaryOfIdAndCancelToken.Add(currentId, new CancellationTokenSource());

            stopwatch.Start();
            var buildTask = Task.Run(() =>
            {
                return reportBuilder.Build(dictionaryOfIdAndCancelToken[currentId].Token);
            }, dictionaryOfIdAndCancelToken[currentId].Token);

            var result = new byte[0];
            try
            {
                 result = await buildTask;
            }
            catch (ReportFailedException)
            {
                reporter.ReportError(currentId);
            }
            stopwatch.Stop();

            if (!dictionaryOfIdAndCancelToken[currentId].IsCancellationRequested && !buildTask.IsFaulted)
            {
                CreateReport(currentId, result, stopwatch.ElapsedMilliseconds);
            }

            return currentId;
        }

        private void CreateReport(int id, byte[] data, long milliseconds)
        {
            if (milliseconds / 1000 < 30)
            {
                reporter.ReportSuccess(data, id);
            }
            else
            {
                reporter.ReportTimeout(id);
            }
        }

        [HttpPost]
        public void Stop([FromBody] StopRequest request)
        {
            dictionaryOfIdAndCancelToken[request.Id].Cancel();
        }
    }
}
