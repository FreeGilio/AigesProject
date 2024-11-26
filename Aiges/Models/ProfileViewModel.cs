namespace Aiges.MVC.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool Admin { get; set; }
        public List<ProjectDetailsViewModel> Projects { get; set; }
    }
}
