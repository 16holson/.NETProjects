namespace Hangman.Server.CustomClasses
{
    public class User
    {
        public string Username;
        public string HashedPass;
        public string Salt;
        public int Score;

        public User(string usrname, string hashedpass, string salt)
        {
            Username = usrname; 
            HashedPass = hashedpass;    
            Salt = salt;
            Score = -1;
        }

    }
}
