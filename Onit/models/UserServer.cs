namespace Onit.models
{
    public class UserServer
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ServerId { get; set; }
        public Server Server { get; set; }
    }
}