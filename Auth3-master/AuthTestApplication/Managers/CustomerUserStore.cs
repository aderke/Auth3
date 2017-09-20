using System;
using AuthTestApplication.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace AuthTestApplication.Managers
{
    public class CustomerUserStore<T> : IUserStore<T> where T : ApplicationUser
    {
        Task IUserStore<T, string>.CreateAsync(T user)
        {
            //Create /Register New User
            throw new NotImplementedException();
        }

        Task IUserStore<T, string>.DeleteAsync(T user)
        {
            //Delete User
            throw new NotImplementedException();
        }

        Task<T> IUserStore<T, string>.FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task<T> IUserStore<T, string>.FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<T, string>.UpdateAsync(T user)
        {
            //Update User Profile
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}