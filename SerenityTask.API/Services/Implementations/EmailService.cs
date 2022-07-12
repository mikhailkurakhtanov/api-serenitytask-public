using System;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using SerenityTask.API.Extensions;
using SerenityTask.API.Components;
using SerenityTask.API.Models.Client.Authentication;
using SerenityTask.API.Models.Server;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Authentication;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SerenityTaskDbContext _dbContext;

        public EmailService(SerenityTaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SendRegisterConfirmationEmail(Register formData)
        {
            var createdUser = _dbContext.Users.FirstOrDefault(x => x.Email == formData.Email);
            var confirmationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-");

            var confirmationLink = Constants.ApiUrl + "user/confirm-account?userId="
                + createdUser.Id + "&confirmationToken=" + confirmationToken;

            var newConfirmationTokenInstance = GetConfirmationToken(confirmationToken,
                TokenType.RegisterConfirmation, createdUser);

            var emailData = new Email
            {
                Subject = Constants.RegisterConfirmationMail,
                Body = new TextPart("html")
                {
                    Text =
                        $"<p>Click on this link to activate your account (the link will expire at " +
                        newConfirmationTokenInstance.ExpirationDate.ToString("ddd, dd MMM yyy hh:mm tt") +
                        " UTC):<br/>" + confirmationLink + "</p>" +
                        $"<p>You will be redirected to the app page immediately.</p><hr>" +
                        $"<p>This message was generated automatically. Don't reply to this.</p>"
                },
                RecipientEmail = formData.Email,
                SenderEmailLogin = Constants.NoReplyEmailLogin,
                SenderEmailPassword = Constants.NoReplyEmailPassword,
                SenderInfo = new MailboxAddress(Constants.CompanyName, Constants.NoReplyEmailLogin)
            };

            CreateAndSendEmail(emailData);

            _dbContext.ConfirmationTokens.Add(newConfirmationTokenInstance);
            _dbContext.SaveChanges();
        }

        public void SendWelcomeEmail(string userEmail, string userName)
        {
            var emailData = new Email
            {
                Subject = "Welcome, " + userName + " | SerenityTask",
                Body = new TextPart("html")
                {
                    Text =
                        $"<h2>Hello!</h2>" +
                        $"<p>Thank you for participating in pre-alpha testing of the SerenityTask app and for your interest in the service.</p>" +
                        $"<p>We are glad to see you among the pioneers of our application! " +
                        $"You are making an invaluable contribution to the development of SerenityTask.</p>" +
                        $"<p>Don't forget to check the list of changes sometimes (the light bulb icon in the site's menu) or in our community in Reddit. " +
                        $"Because of this, you will not miss any important updates of SerenityTask!</p>" +
                        $"<p>If you have any questions about using the application or suggestions for improving SerenityTask, " +
                        $"feel free to write to us at this address - contact@csfullstack.com</p><br/><hr>" +
                        $"<p style=\"font-style: italic; font-weight: bold;\">Best wishes, <br/>Mikhail Kurakhtanov, The Founder of CS FULLSTACK</p>"
                },
                RecipientEmail = userEmail,
                SenderEmailLogin = Constants.PersonalEmailLogin,
                SenderEmailPassword = Constants.PersonalEmailPassword,
                SenderInfo = new MailboxAddress("Mikhail Kurakhtanov", Constants.PersonalEmailLogin)
            };

            CreateAndSendEmail(emailData);
        }

        public async Task SendPasswordRecoveryConfirmationEmail(ChangeAccountPasswordRequest request)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == request.UserEmail);
            var confirmationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-");

            var confirmationLink = Constants.ApiUrl + "user/confirm-password?userId=" + user.Id
                + "&newPasswordHash=" + request.NewPassword.GetPasswordHash() + "&confirmationToken=" + confirmationToken;

            var newConfirmationTokenInstance = GetConfirmationToken(confirmationToken, TokenType.PasswordConfirmationOnChange, user);

            var emailData = new Email
            {
                Subject = "Password Recovery Confirmation | SerenityTask",
                Body = new TextPart("html")
                {
                    Text =
                        $"<p>You decided to recover you password for '" + request.UserEmail + "' account.<br/>" +
                        $"<p>Click on this link to apply your's account changes (the link will expire at " +
                        newConfirmationTokenInstance.ExpirationDate.ToString("ddd, dd MMM yyy hh:mm tt") + " UTC) - " +
                        confirmationLink + $"</p><p>You will be redirected to the app page immediately.</p><hr>" +
                        $"<p>This message was generated automatically. Don't reply to this.</p>"
                },
                RecipientEmail = request.UserEmail,
                SenderEmailLogin = Constants.NoReplyEmailLogin,
                SenderEmailPassword = Constants.NoReplyEmailPassword,
                SenderInfo = new MailboxAddress(Constants.CompanyName, Constants.NoReplyEmailLogin)
            };

            CreateAndSendEmail(emailData);

            _dbContext.ConfirmationTokens.Add(newConfirmationTokenInstance);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SettingsNotification> SendPasswordConfirmationEmail(User currentUser, string newUserPassword)
        {
            var newPasswordHash = newUserPassword.GetPasswordHash();
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-");

            var confirmationLink = Constants.ApiUrl + "account/confirm-password?newPassword=" + newPasswordHash
                + "&confirmationToken=" + token + "&userId=" + currentUser.Id;

            var newConfirmationToken = GetConfirmationToken(token, TokenType.PasswordConfirmationOnChange, currentUser);

            var emailData = new Email
            {
                Subject = "Password Changing Confirmation | SerenityTask",
                Body = new TextPart("html")
                {
                    Text =
                        $"<p>You decided to change you password for '" + currentUser.Email + "' account.<br/>" +
                        $"<p>Click on this link to apply your's account changes (the link will expire at " +
                        newConfirmationToken.ExpirationDate.ToString("ddd, dd MMM yyy hh:mm tt") + " UTC) - " +
                        confirmationLink + $"</p><p>You will be redirected to the app page immediately.</p><hr>" +
                        $"<p>This message was generated automatically. Don't reply to this.</p>"
                },
                RecipientEmail = currentUser.Email,
                SenderEmailLogin = Constants.NoReplyEmailLogin,
                SenderEmailPassword = Constants.NoReplyEmailPassword,
                SenderInfo = new MailboxAddress(Constants.CompanyName, Constants.NoReplyEmailLogin)
            };

            CreateAndSendEmail(emailData);

            _dbContext.ConfirmationTokens.Add(newConfirmationToken);

            DeleteSettingsNotifications(SettingsNotificationType.Password);

            var newNotification = new SettingsNotification
            {
                Result = true,
                Type = SettingsNotificationType.Password,
                Message = "Confirmation link has been sent to " + currentUser.Email,
                User = currentUser
            };

            _dbContext.SettingsNotifications.Add(newNotification);
            await _dbContext.SaveChangesAsync();

            return newNotification;
        }

        public async Task<SettingsNotification> SendEmailConfirmationEmail(User currentUser, string newUserEmail)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-");

            var confirmationLink = Constants.ApiUrl + "account/confirm-email?newEmail=" + newUserEmail
                + "&confirmationToken=" + token + "&userId=" + currentUser.Id;

            var newConfirmationToken = GetConfirmationToken(token, TokenType.EmailConfirmationOnChange, currentUser);

            var emailData = new Email
            {
                Subject = "Email Changing Confirmation | SerenityTask",
                Body = new TextPart("html")
                {
                    Text =
                        $"<p>You decided to change you email for '" + currentUser.Email + "' account.<br/>" +
                        $"<p>This is your new email: " + newUserEmail + "</p>" +
                        $"<p>Click on this link to apply your's account changes (the link will expire at " +
                        newConfirmationToken.ExpirationDate.ToString("ddd, dd MMM yyy hh:mm tt") + " UTC) - " +
                        confirmationLink + $"</p><p>You will be redirected to the app page immediately.</p><hr>" +
                        $"<p>This message was generated automatically. Don't reply to this.</p>"
                },
                RecipientEmail = newUserEmail,
                SenderEmailLogin = Constants.NoReplyEmailLogin,
                SenderEmailPassword = Constants.NoReplyEmailPassword,
                SenderInfo = new MailboxAddress(Constants.CompanyName, Constants.NoReplyEmailLogin)
            };

            CreateAndSendEmail(emailData);

            _dbContext.ConfirmationTokens.Add(newConfirmationToken);

            DeleteSettingsNotifications(SettingsNotificationType.Email);

            var newNotification = new SettingsNotification
            {
                Result = true,
                Type = SettingsNotificationType.Email,
                Message = "Confirmation link has been sent to " + newUserEmail,
                User = currentUser
            };

            _dbContext.SettingsNotifications.Add(newNotification);
            await _dbContext.SaveChangesAsync();

            return newNotification;
        }

        private static ConfirmationToken GetConfirmationToken(string token, TokenType tokenType, User createdUser)
        {
            var newActivationToken = new ConfirmationToken
            {
                Token = token,
                Type = tokenType,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddHours(1),
                User = createdUser
            };

            return newActivationToken;
        }

        private static void CreateAndSendEmail(Email emailData)
        {
            var newMessage = new MimeMessage();
            var smtpClient = new SmtpClient();

            newMessage.From.Add(emailData.SenderInfo);
            newMessage.To.Add(MailboxAddress.Parse(emailData.RecipientEmail));
            newMessage.Subject = emailData.Subject;
            newMessage.Body = emailData.Body;

            smtpClient.Connect(Constants.EmailServer, Constants.EmailPort, true);
            smtpClient.Authenticate(emailData.SenderEmailLogin, emailData.SenderEmailPassword);
            smtpClient.Send(newMessage);
            smtpClient.Disconnect(true);
            smtpClient.Dispose();
        }

        private void DeleteSettingsNotifications(SettingsNotificationType settingsNotificationType)
        {
            var existingSettingsNotifications = _dbContext.SettingsNotifications.Where(x => x.Type == settingsNotificationType);
            if (existingSettingsNotifications.Any())
            {
                _dbContext.SettingsNotifications.RemoveRange(existingSettingsNotifications);
            }
        }
    }
}
