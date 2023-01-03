using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionService.Web.Models.DAO;
using Dapper;

namespace SubscriptionService.Web.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUser(Guid id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserByEmail(string emailAddress);
        Task InsertUser(User user);
    }
    public class UserRepository : IUserRepository
    {
        private readonly IDBConnectionProvider _dBConnectionProvider;
        public UserRepository(IDBConnectionProvider dBConnectionProvider)
        {
            _dBConnectionProvider = dBConnectionProvider;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var sql = @"SELECT id, externaluserid, name, email, salary, expenses, datecreated 
                        FROM users u";

            using (var cnn = await _dBConnectionProvider.CreateDBConnection())
            {
                return (await cnn.QueryAsync<User>(sql));
            }
        }

        public async Task<User> GetUser(Guid id)
        {
            var sql = @"SELECT id, externaluserid, name, email, salary, expenses, datecreated 
                        FROM users u where u.externaluserid = :userId";

            using (var cnn = await _dBConnectionProvider.CreateDBConnection())
            {
                return (await cnn.QueryAsync<User>(sql, new { userId = id })).FirstOrDefault();
            }
        }

        public async Task<User> GetUserByEmail(string emailAddress)
        {
            var sql = @"SELECT id, externaluserid, name, email, salary, expenses, datecreated 
                        FROM users u where u.email = :emailAddress";

            using (var cnn = await _dBConnectionProvider.CreateDBConnection())
            {
                return (await cnn.QueryAsync<User>(sql, new { emailAddress = emailAddress })).FirstOrDefault();
            }
        }

        public async Task InsertUser(User user)
        {
            DynamicParameters MapParams()
            {
                var param = new DynamicParameters();
                param.Add("externaluserid", user.ExternalUserId);
                param.Add("name", user.Name);
                param.Add("email", user.Email);
                param.Add("salary", user.Salary);
                param.Add("expenses", user.Expenses);
                param.Add("datecreated", user.DateCreated);
                return param;
            }

            var sql = @"INSERT INTO users (externaluserid, name, email, salary, expenses, datecreated)
                        values (:externaluserid, :name, :email, :salary, :expenses, :datecreated)";
            using (var cnn = await _dBConnectionProvider.CreateDBConnection())
            {
                await cnn.ExecuteAsync(sql, MapParams());
            }            
        }
    }
}
