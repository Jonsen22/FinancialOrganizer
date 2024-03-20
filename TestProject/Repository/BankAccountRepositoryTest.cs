using Microsoft.EntityFrameworkCore;
using Organizer.Context;
using Organizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Repository
{
    public class BankAccountRepositoryTest
    {
        protected async Task<AppDbContext> GetDbContext()
        {
            //var options = new DbContextOptionsBuilder<AppDbContext>()
            //    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            //    .Options;

            //var databaseContext = new AppDbContext(options);
            //databaseContext.Database.EnsureCreated();

            //if (await databaseContext.BankAccounts.CountAsync() < 0)
            //{
            //    for (int i = 0; i < 10; i++)
            //    {

            //        databaseContext.BankAccounts.Add(
            //        new BankAccount()
            //        {
            //            BankAccountId = 1,
            //            Balance = 1500,
            //            Name = "Nubank",
            //            UserId = "05f60a81-9ad3-485a-ae06-f242d05badc4"
            //        }) ;
            //        await databaseContext.SaveChangesAsync();
            //    }
            //}
            var options = new DbContextOptionsBuilder<AppDbContext>()
       .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
       .Options;

            var databaseContext = new AppDbContext(options);

            if (!await databaseContext.BankAccounts.AnyAsync())
            {
                var userId = "05f60a81-9ad3-485a-ae06-f242d05badc4";
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.BankAccounts.Add(
                        new BankAccount()
                        {
                            BankAccountId = i + 1, 
                            Balance = 1500,
                            Name = "Nubank",
                            UserId = userId
                        });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }


    }
}
