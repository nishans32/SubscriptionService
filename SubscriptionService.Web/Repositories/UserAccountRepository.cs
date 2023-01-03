using Dapper;
using SubscriptionService.Web.Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionService.Web.Repositories
{
    public interface IUserAccountRepository
    {
        Task<UserAccount> GetAccounts(Guid userId);
        //todo Add GetAccount
        Task InsertAccount(Account userAccount);
    }

    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IDBConnectionProvider _dbConnectionProvider;

        public UserAccountRepository(IDBConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<UserAccount> GetAccounts(Guid userId)
        {
            var sql = @"SELECT u.id, u.externaluserid, u.name, u.email, u.salary, u.expenses, u.datecreated,
                               ua.id, ua.accounttype, ua.userid, ua.loanamount, ua.interestrate, ua.repaymentamount, ua.repaymentfrequency, ua.datecreated
                        FROM users u 
                        INNER JOIN useraccount ua on u.id = ua.userid
                        where u.externaluserid = :userId";

            using (var cnn = await _dbConnectionProvider.CreateDBConnection())
            {
                UserAccount userAccount = null; 
                var accounts = new List<Account>();
                var result = await cnn.QueryAsync<User, Account, UserAccount>(sql, (user, account) => 
                {
                    if (userAccount == null)
                    {
                        userAccount = new UserAccount();
                        userAccount.User = user;

                        userAccount.Accounts = new List<Account>();
                    }

                    userAccount.Accounts.Add(account);

                    return userAccount;
                } ,new { userId = userId },
                splitOn: "id");
                return result.FirstOrDefault();
            }
        }

        public async Task InsertAccount(Account userAccount)
        {
            DynamicParameters MapParams()
            {
                var param = new DynamicParameters();
                param.Add("userid", userAccount.UserId);
                param.Add("accounttype", userAccount.AccountType);
                param.Add("loanamount", userAccount.LoanAmount);
                param.Add("interestrate", userAccount.InterestRate);
                param.Add("repaymentamount", userAccount.RepaymentAmount);
                param.Add("repaymentfrequency", userAccount.RepaymentFrequency);
                param.Add("datecreated", userAccount.DateCreated);
                return param;
            }

            var sql = @"INSERT INTO useraccount(accounttype, userid, loanamount, interestrate, repaymentamount, repaymentfrequency, datecreated)
                        values (:accounttype, :userid, :loanamount, :interestrate, :repaymentamount, :repaymentfrequency, :datecreated)";
            using (var cnn = await _dbConnectionProvider.CreateDBConnection())
            {
                await cnn.ExecuteAsync(sql, MapParams());
            }
        }
    }
}