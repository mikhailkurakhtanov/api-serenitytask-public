using MimeKit;

namespace SerenityTask.API.Models.Server
{
    public class Email
    {
        public string Subject { get; set; }

        public TextPart Body { get; set; }

        public string SenderEmailLogin { get; set; }

        public string SenderEmailPassword { get; set; }

        public string RecipientEmail { get; set; }

        public MailboxAddress SenderInfo { get; set; }
    }
}
