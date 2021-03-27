using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBank_A3.Models;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

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



    }
}
