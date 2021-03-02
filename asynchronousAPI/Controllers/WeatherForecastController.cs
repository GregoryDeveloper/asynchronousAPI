using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace asynchronousAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static Object lockObj = new Object();
        public WeatherForecastController()
        {
        }

        [HttpGet("milliseconds/{milliseconds}/get-await")]
        public async Task<IActionResult> GetAwait([FromRoute] int milliseconds)
        {
            var result = await GetAwaitResult(milliseconds);
            return Ok(result);
        }

        private async Task<bool> GetAwaitResult(int milliseconds)
        {
            int threadIdBefore = Thread.CurrentThread.ManagedThreadId;
            await Task.Delay(milliseconds);
            int threadIdAfter = Thread.CurrentThread.ManagedThreadId;
            return threadIdBefore == threadIdAfter;
        }

        [HttpGet("milliseconds/{milliseconds}/get-wait")]
        public IActionResult GetWait([FromRoute] int milliseconds)
        {
            Console.WriteLine("request received for wait");
            var result = GetWaitResult(milliseconds);
            return Ok(result);
        }
        private bool GetWaitResult(int milliseconds)
        {
            int threadIdBefore =  Thread.CurrentThread.ManagedThreadId;
            Task.Delay(milliseconds).Wait();
            int threadIdAfter = Thread.CurrentThread.ManagedThreadId;
            return threadIdBefore == threadIdAfter;
        }


        [HttpGet("milliseconds/{milliseconds}/number-of-iteration/{numberOfIteration}/get-when-all")]
        public async Task<IActionResult> GetWhenAll([FromRoute] int milliseconds, [FromRoute] int numberOfIteration)
        {
            bool result = await DoTasksWhenAll(milliseconds, numberOfIteration);
            return Ok(result);
        }

        private async Task<bool> DoTasksWhenAll(int milliseconds, int numberOfIteration)
        {
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < numberOfIteration; i++)
            {
                var task = DoDomething(milliseconds);
                tasks.Add(task);
            }
            int threadIdBefore = ShowThreadInformation();
            await Task.WhenAll(tasks);
            int threadIdAfter = ShowThreadInformation();
            return threadIdBefore == threadIdAfter;
        }

        [HttpGet("milliseconds/{milliseconds}/number-of-iteration/{numberOfIteration}/get-wait-all")]
        public IActionResult GetWait([FromRoute] int milliseconds, [FromRoute] int numberOfIteration)
        {
            bool result = DoTasksWaitAll(milliseconds, numberOfIteration);
            return Ok(result);
        }

        private bool DoTasksWaitAll(int milliseconds, int numberOfIteration)
        {
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < numberOfIteration; i++)
            {
                var task = DoDomething(milliseconds);
                tasks.Add(task);
            }

            int threadIdBefore = ShowThreadInformation();
            Task.WaitAll(tasks.ToArray());
            int threadIdAfter = ShowThreadInformation();

            return threadIdBefore == threadIdAfter;
        }
        private Task DoDomething(int milliseconds)
        {
            return Task.Delay(milliseconds);
        }
        private static int ShowThreadInformation()
        {
            String msg = "";
            Thread thread = Thread.CurrentThread;
            lock (lockObj)
            {
                msg = String.Format("   Thread ID: {0}\n", thread.ManagedThreadId);
            }
            //Console.WriteLine(msg);
            return thread.ManagedThreadId;
        }
    }
}
