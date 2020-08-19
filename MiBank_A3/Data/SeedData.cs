using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MiBank_A3.Models
{
    public partial class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new MiBankContext(serviceProvider.GetRequiredService<DbContextOptions<MiBankContext>>());
            if (context.Customers.Any())
            {
                return;
            }

            //add payees

            context.Payee.Add(new Payee
            {
                PayeeName = "Telstro",
                Address = "3 Sydney rd",
                City = "Sydney",
                State = State.NSW,
                Postcode = "1234",
                Phone = "1234 1234"
            });
            context.Payee.Add(new Payee
            {
                PayeeName = "Optas",
                Address = "3 Adelaide rd",
                City = "Adelaide",
                State = State.SA,
                Postcode = "3456",
                Phone = "3456 4567"
            });
            context.SaveChanges();

            //add logins, customers etc from web service

            var logins = deserialize<DTA_Login[]>("https://titan.csit.rmit.edu.au/~e07582/wdt/services/logins/");
            var customers = deserialize<DTA_Customer[]>("https://titan.csit.rmit.edu.au/~e07582/wdt/services/customers/");

            foreach (var c in customers)
            {
                
                var currentCustomer = context.Customers.Add(new Customer {
                    CustomerName = c.Name,
                    Address = c.Address,
                    City = c.City,
                    PostCode = c.PostCode}
                );

                context.SaveChanges();     //need to save changes for every customer to make the ID update

                //add a login for each customer
                var l = logins.Single(x => x.CustomerID == c.CustomerID);
                context.LoginDetails.Add(new Login
                {
                    LoginName = l.LoginID,
                    CustomerID = currentCustomer.Entity.CustomerId,
                    PasswordHash = l.PasswordHash
                });

                context.SaveChanges();


                //add accounts and transactions for each customer
                foreach (var a in c.Accounts)
                {

                    //account type
                    Account.Type accountType;
                    if(a.AccountType == 0)
                    {
                        accountType = Account.Type.Savings;
                    }
                    else
                    {
                        accountType = (Account.Type)a.AccountType;
                    }

                    var currentAccount = context.Accounts.Add(new Account
                    {
                        AccountType = accountType,
                        CustomerId = currentCustomer.Entity.CustomerId,
                        Balance = a.Balance
                    }).Entity;
                    context.SaveChanges();

                    foreach (var t in a.Transactions)
                    {
                        context.BankTransactions.Add(new Transaction
                        {
                            TransactionTime = t.TransactionTimeUTC,
                            Amount = t.Amount,
                            AccountId = currentAccount.AccountId,
                            TransactionType = TransactionType.Deposit
                        });
                    }
                    context.SaveChanges();
                }

            }
        }


        private static T deserialize<T>(string url)
        {
            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.AutomaticDecompression = DecompressionMethods.All;
            using var res = req.GetResponse();
            using var reader = new System.IO.StreamReader(res.GetResponseStream());
            var settings = new JsonSerializerSettings();
            settings.DateFormatString = "MM/dd/yyyy hh:mm:ss tt";
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(reader.ReadToEnd(), settings);
        }




    }




}
