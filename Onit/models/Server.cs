namespace Onit.models
{
    public class Server
    {
        public int Id { get; set; }
        public string Games { get; set; }
        public int Players { get; set; }

        public ICollection<UserServer> UserServers { get; set; }
    }
}