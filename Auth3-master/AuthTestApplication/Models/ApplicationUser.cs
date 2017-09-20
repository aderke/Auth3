using Microsoft.AspNet.Identity;

namespace AuthTestApplication.Models
{
    public class ApplicationUser : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public ApplicationRole Role { get; set; }
    }
}