using System.Collections.Generic;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            this._context = context;
        }

        public void SeedUsers(){
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            // convert text to object
            var users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(userData);

            foreach(var user in users){
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UserName = user.UserName.ToLower();

                _context.Users.Add(user);               
            }
            _context.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // eventhough iplement IDisposable we still need to use using{}
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // generate salt key
                passwordSalt = hmac.Key;
                // use salt key for hashing
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}