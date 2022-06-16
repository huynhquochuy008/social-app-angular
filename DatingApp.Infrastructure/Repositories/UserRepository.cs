﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DatingApp.Application.Interfaces;
using DatingApp.Core.Entities;
using DatingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<AppUser> GetAll()
        {
            return _context.Users.ToList();
        }

        public AppUser GetById(int userID)
        {
            return _context.Users.Find(userID);
        }

        public void Insert(AppUser user)
        {
            _context.Users.Add(user);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
        public void Delete(int userID)
        {
            AppUser user = _context.Users.Find(userID);
            _context.Users.Remove(user);
        }

        public Task<bool> CheckEmailExist(string userEmail)
        {
            return _context.Users.AnyAsync(x => x.Email == userEmail.ToLower());
        }

        public Task<bool> CheckUsernameExist(string username)
        {
            return _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
