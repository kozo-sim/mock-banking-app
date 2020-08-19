using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Models;

namespace MiBank_A3.Models
{
    public class MiBankContext : DbContext
    {
        public MiBankContext(DbContextOptions<MiBankContext> options)
            : base(options)
        {
        }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> BankTransactions { get; set; }
        public DbSet<Login> LoginDetails { get; set; }
        public DbSet<Payee> Payee { get; set; }
        public DbSet<BillPay> BillPay { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(k => k.CustomerId);

            modelBuilder.Entity<Transaction>()
                .HasOne(a => a.Account)
                .WithMany(c => c.Transactions)
                .HasForeignKey(k => k.AccountId);

            modelBuilder.Entity<Customer>()
                .HasOne<Login>(l => l.LoginDetails)
                .WithOne(c => c.Customer)
                .HasForeignKey<Login>(k => k.CustomerID);

            //works, but doesn't help us find receivedTransactions
            //modelBuilder.Entity<BankTransaction>()
            //    .HasOne(a => a.TransferTarget)
            //    .WithMany()
            //    .HasForeignKey(k => k.TransferTargetId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //2nd attempt
            modelBuilder.Entity<Transaction>()
                .HasOne(a => a.TransferTarget)
                .WithMany(a => a.ReceivedTransactions)
                .HasForeignKey(k => k.TransferTargetId)
                .OnDelete(DeleteBehavior.Restrict);

            //login
            modelBuilder.Entity<Login>()
                .HasIndex(l => l.LoginName)
                .IsUnique();

            //bills
            modelBuilder.Entity<BillPay>()
                .HasOne(b => b.Account)
                .WithMany(a => a.Bills)
                .HasForeignKey(b => b.AccountId);

            modelBuilder.Entity<BillPay>()
                .HasOne(b => b.Payee)
                .WithMany(a => a.Bills)
                .HasForeignKey(b => b.PayeeId);

            modelBuilder.Entity<Payee>()
                .HasIndex(l => l.PayeeName)
                .IsUnique();

        }




        public ValueTask<Customer> GetCustomer(int? CustomerId)
        {
            return Customers.FindAsync(CustomerId);
        }

        public Task<Customer> GetCustomerWithAccounts(int? CustomerId)
        {
            return Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CustomerId == CustomerId);
        }

        public Task<Account> GetAccount(int? CustomerId, int? accountId)
        {
            return Accounts
                .FirstOrDefaultAsync(
                x => x.AccountId == accountId
                && x.CustomerId == CustomerId);
        }

        public Task<Account> GetAccountWithTransactions(int? CustomerId, int? accountId)
        {
            return Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(
                x => x.AccountId == accountId
                && x.CustomerId == CustomerId);
        }

        //NOTE: includes transactions sent to this account
        public List<Transaction> GetAllTransactions(int? accountId)
        {
            return BankTransactions
                .Where(t => t.TransferTargetId == accountId || t.AccountId == accountId).ToList();
        }

        //TODO; use lazy loading
        public List<BillPay> GetBills(int? CustomerId)
        {
            return BillPay.Where(
                    b => b.Account.Customer.CustomerId == CustomerId
                    )
                .Include(b => b.Account)
                .Include(b => b.Payee)
                    .OrderByDescending(b => b.BillPayId)
                    .ToList();
        }


        public List<BillPay> GetAllBills()
        {
            return BillPay
                .Include(b => b.Account)
                .Include(b => b.Payee)
                    .OrderByDescending(b => b.BillPayId)
                    .ToList();
        }

        public Task<BillPay> GetBill(int? CustomerId, int? id)
        {
            return BillPay
                .FirstOrDefaultAsync(
                b => b.BillPayId == id
                && b.Account.Customer.CustomerId == CustomerId);
        }

        public List<Payee> GetAllPayees(int? CustomerId)
        {
            return Payee.ToList();
        }

        public string GetUsername(int? CustomerId)
        {
            return LoginDetails.FirstOrDefault(l => l.CustomerID == CustomerId).LoginName;
        }

        private async Task<Login> GetLogin(string username)
        {
            return await LoginDetails
                .SingleOrDefaultAsync(l => l.LoginName == username);
        }

        private bool VerifyHash(string hash, string password)
        {
            return SimpleHashing.PBKDF2.Verify(hash, password);
        }

        public async Task<int> Login(string username, string password)
        {
            if (password == null)
            {
                return -1;
            }
            var login = await GetLogin(username);
            if (login == null || !VerifyHash(login.PasswordHash, password))
            {
                return -1;
            }

            return login.CustomerID;
        }

        public async Task<bool> UpdateLogin(int customerId, string oldPassword, string newUsername = null, string newPassword = null)
        {
            if (oldPassword == null)
            {
                return false;
            }
            var login = await LoginDetails
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
            await SaveChangesAsync();
            return true;
        }
    }
}
