using Microsoft.AspNetCore.Identity;

namespace WebCinema.ViewModel.Users
{
    public class ChangeUserRoleViewModel
    {
        public IList<string> UserRoles { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }  

        public List<IdentityRole> AllRoles { get; set; }

        public ChangeUserRoleViewModel()
        {
            UserRoles = new List<string>();
            AllRoles = new List<IdentityRole>();
        }
    }
}
