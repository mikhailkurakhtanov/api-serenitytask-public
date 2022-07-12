namespace SerenityTask.API.Models.Requests.Authentication
{
    public class ChangeAccountPasswordRequest
    {
        public string UserEmail { get; set; }

        public string NewPassword { get; set; }
    }
}