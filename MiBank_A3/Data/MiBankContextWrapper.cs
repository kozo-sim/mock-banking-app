using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Models;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace MiBank_A3.Data
{
    //This class exists to fully decouple controllers from the database
    //Controllers should instantiate this in their constructor
    public class MiBankContextWrapper : IDisposable
    {
        private readonly MiBankContext _context;

        public MiBankContextWrapper(MiBankContext context)
        {
            _context = context;
        }

        public MiBankContextWrapper(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MiBankContext>();
        }

        public ValueTask<Customer> GetCustomer(int? CustomerId)
        {
            return _context.Customers.FindAsync(CustomerId);
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers.ToList();
        }

        public Task<Customer> GetCustomerWithAccounts(int? CustomerId)
        {
            return _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CustomerId == CustomerId);
        }

        public void UpdateCustomer(Customer customer)
        {
            _context.Update(customer);
            SaveChangesAsync();
        }


        public Task<Account> GetAccount(int? CustomerId, int? accountId)
        {
            return _context.Accounts
                .FirstOrDefaultAsync(
                x => x.AccountId == accountId
                && x.CustomerId == CustomerId);
        }

        public Task<Account> GetAccountWithTransactions(int? CustomerId, int? accountId)
        {
            return _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(
                x => x.AccountId == accountId
                && x.CustomerId == CustomerId);
        }

        //NOTE: includes transactions sent to this account
        public List<Transaction> GetAllAccountTransactions(int? accountId)
        {
            return _context.BankTransactions
                .Where(t => t.TransferTargetId == accountId || t.AccountId == accountId).ToList();
        }

        //NOTE: includes transactions sent to this customer's accounts
        public List<Transaction> GetAllCustomerTransactions(int customerId)
        {
            return _context.BankTransactions
                .Where(t => t.Account.CustomerId == customerId || t.TransferTarget.CustomerId == customerId).ToList();
        }

        public void CreateTransaction(Transaction transaction)
        {
            _context.BankTransactions.Add(transaction);
            SaveChangesAsync();
        }

        //TODO; use lazy loading
        public List<BillPay> GetBills(int? CustomerId)
        {
            return _context.BillPay.Where(
                    b => b.Account.Customer.CustomerId == CustomerId
                    )
                .Include(b => b.Account)
                .Include(b => b.Payee)
                    .OrderByDescending(b => b.BillPayId)
                    .ToList();
        }


        public List<BillPay> GetAllBills()
        {
            return _context.BillPay
                .Include(b => b.Account)
                .Include(b => b.Payee)
                    .OrderByDescending(b => b.BillPayId)
                    .ToList();
        }

        public void CreateBill(int? CustomerId, BillPay bill)
        {
            if (CustomerId == bill.Account.CustomerId)
            {
                _context.BillPay.AddAsync(bill);
            }
            SaveChangesAsync();
        }

        public async void DeleteBill(int? CustomerId, int? BillId)
        {
            var targetBill = await GetBill(CustomerId, BillId);
            _context.BillPay.Remove(targetBill);
            SaveChangesAsync();
        }


        public Task<BillPay> GetBill(int? CustomerId, int? id)
        {
            return _context.BillPay
                .FirstOrDefaultAsync(
                b => b.BillPayId == id
                && b.Account.Customer.CustomerId == CustomerId);
        }

        public void UpdateBill(int? CustomerId, BillPay bill)
        {
            _context.Update(bill);
        }

        public Task<Payee> GetPayee(int? PayeeId)
        {
            return _context.Payee
                .FirstOrDefaultAsync(
                p => p.PayeeId == PayeeId);
        }

        public List<Payee> GetAllPayees()
        {
            return _context.Payee.ToList();
        }

        public string GetUsername(int? CustomerId)
        {
            return _context.LoginDetails.FirstOrDefault(l => l.CustomerID == CustomerId).LoginName;
        }

        private async Task<Login> GetLogin(string username)
        {
            return await _context.LoginDetails
                .SingleOrDefaultAsync(l => l.LoginName == username);
        }

        private bool VerifyHash(string hash, string password)
        {
            return SimpleHashing.PBKDF2.Verify(hash, password);
        }


        //variables to be used in Login() method
        private static Dictionary<int, int> failedLoginAttempts;
        const int MAX_LOGIN_ATTEMPTS = 3;

        public async Task<int> Login(string username, string password)
        {
            if (failedLoginAttempts == null)
            {
                failedLoginAttempts = new Dictionary<int, int>();
            }
            if (password == null)
            {
                return (int)loginResult.NO_PASSWORD;
            }
            var login = await GetLogin(username);
            if (login == null)
            {
                return (int)loginResult.NO_USER;
            }

            if (failedLoginAttempts.GetValueOrDefault(login.CustomerID) >= MAX_LOGIN_ATTEMPTS)
            {
                return (int)loginResult.MAX_ATTEMPTS;
            }
            if (!VerifyHash(login.PasswordHash, password))
            {
                failedLoginAttempts.TryAdd(login.CustomerID, 0);
                failedLoginAttempts[login.CustomerID]++;
                return (int)loginResult.WRONG_PASSWORD;
            }

            return login.CustomerID;
        }

        //called once a minute from LoginTimerService
        public void ResetLoginTimers()
        {
            //zero out failed attempts
            failedLoginAttempts = new Dictionary<int, int>();
        }

        //called from api
        public bool LockCustomerAccount(int customerId)
        {
            if (this.GetCustomer(customerId) != null)
            {
                failedLoginAttempts[customerId] = int.MaxValue;
                return true;
            }
            return false;
        }



        public async Task<bool> UpdateLogin(int customerId, string oldPassword, string newUsername = null, string newPassword = null)
        {
            if (oldPassword == null)
            {
                return false;
            }
            var login = await _context.LoginDetails
                .SingleOrDefaultAsync(l => l.CustomerID == customerId);
            if (login == null || !VerifyHash(login.PasswordHash, oldPassword))
            {
                return false;
            }
            if (newUsername != null)
            {
                login.LoginName = newUsername;
            }
            if (newPassword != null)
            {
                login.PasswordHash = SimpleHashing.PBKDF2.Hash(newPassword);
            }
            SaveChangesAsync();
            return true;
        }

        public async void SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
