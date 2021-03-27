using MiBank_A3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiBank_A3.Data
{
    public class LoginTimerService : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public LoginTimerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(PayBills, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void PayBills(object state)
        {
            using var context = new MiBankContextWrapper(_serviceProvider);
            context.ResetLoginTimers();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }


        void IDisposable.Dispose()
        {
            _timer.Dispose();
        }
    }
}
