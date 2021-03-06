using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _ctx;
        public DatingRepository(DataContext ctx)
        {
            this._ctx = ctx;

        }

        public void Add<T>(T entity) where T : class
        {
            // save only in memory not yet commit to DB
            _ctx.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _ctx.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _ctx.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _ctx.Users.Include(p => p.Photos).ToListAsync();
            return users;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _ctx.Users
                        .Include(p => p.Photos)
                        .OrderByDescending(u => u.LastActive).AsQueryable();

            if(userParams.MinAge != 18 || userParams.MaxAge != 99) {
                var minDateofBirth =  DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDateofBirth =  DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.Id != userParams.UserId && 
                                u.Gender == userParams.Gender && 
                                u.DateOfBirth >= minDateofBirth && 
                                u.DateOfBirth <= maxDateofBirth);
            }
            else {
                users = users.Where(u => u.Id != userParams.UserId && u.Gender == userParams.Gender);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy)) {
                switch (userParams.OrderBy) {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            // commit data in to DB
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<int> CountUser()
        {
            return await _ctx.Users.CountAsync();
        }

        public async Task<Photo> GetPhoto(int id){
            var photo = await _ctx.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoByUserId(int userId) {
            var photo = await _ctx.Photos.Where(p => p.UserId == userId && p.IsMain == true).FirstOrDefaultAsync();
            return photo;
        }
    }
}