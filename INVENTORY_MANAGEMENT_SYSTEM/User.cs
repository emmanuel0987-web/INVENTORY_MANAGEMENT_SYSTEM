using System;

namespace InventoryManagementSystem
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        private string Password { get; set; }

        public User(int id, string username, string fullName, string role, string password)
        {
            Id = id;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            FullName = fullName ?? string.Empty;
            Role = role ?? "Staff";
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public bool Authenticate(string password) => Password == password;

        public override string ToString()
        {
            return string.Format("[{0}] {1} ({2}) - Role: {3}", Id, Username, FullName, Role);
        }
    }
}