namespace WebCinema.Models.Users
{
    public class DeleteUserViewModel
    {
        public string Id { get; set; }
        public IList<string> UserRoles { get; set; }
        public string Email { get; set; }

        public DeleteUserViewModel()
        {
            UserRoles = new List<string>();
        }
    }
}
