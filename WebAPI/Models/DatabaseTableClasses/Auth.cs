
namespace WebAPI.Models.DatabaseTableClasses
{
    public class Auth
    {
        public string token { get; set; }
        public string refresh_token { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}