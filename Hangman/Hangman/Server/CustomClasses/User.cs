namespace Hangman.Server.CustomClasses
{
    public class User
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }   
        public string Salt { get; set; }
        public int HighScore { get; set; }  

        public User(string userName, string passwordHash, string salt)
        {
            this.UserName = userName;   
            this.PasswordHash = passwordHash;
            this.Salt = salt;
        }
    }
}
