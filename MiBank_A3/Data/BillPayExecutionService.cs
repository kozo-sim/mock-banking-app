using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{
    public class BillPayExecutionService : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly IServiceProvider _serviceProvier;

        public BillPayExecutionService(IServiceProvider serviceProvider)
        {
            _serviceProvier = serviceProvider;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(PayBills, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void PayBills(object state)
        {
            using var scope = _serviceProvier.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<MiBankContext>();
            var bills = context.GetAllBills();
            foreach(var bill in bills)
            {
                var resultBag = bill.doScheduledPayment();
                switch (resultBag.transactionResult) 
                {
                    case ScheduledBillPayResult.OK_NOT_PAID:
                    case ScheduledBillPayResult.BLOCKED:
                        //do nothing
                        break;
                    case ScheduledBillPayResult.OK_PAID:
                    case ScheduledBillPayResult.FAIL_NOT_ENOUGH:
                        context.BankTransactions.Add(resultBag.transaction);
                        context.SaveChanges();
                        break;
                    default:
                        throw new ArgumentException("Unhandled value in switch statement");
                }
            }


            context.SaveChanges();
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
