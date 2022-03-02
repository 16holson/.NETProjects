using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WCS.Services;

namespace WCS.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "CS Scholarships - Confirm Your Email",
                "" +
                $"Please confirm your account by clicking this link: <a href='{link}'>Confirm Email</a>." +
                "<br><br><hr>Please do not reply to this message. This email is not monitored.");
        }

        public static Task SendForgotPasswordAsync (this IEmailSender emailSender, string email, string link)
        {

            return emailSender.SendEmailAsync(email, "CS Scholarships - Password Reset Request",
                "We have received a request to reset your password. To reset your password please click " +
                    $"<a href='{link}'>here</a>." +
                    "<br><br><hr>Please do not reply to this message. This email is not monitored.");
        }

        public static Task SendInviteAsync(this IEmailSender emailSender, string email, string fullname, string role, string link)
        {
            return emailSender.SendEmailAsync(email, $"CS Scholarships - Invitation", $"Hello {fullname}, <br>" +
                $"You've been invited to register in the Weber State University CS Scholarship Application system in the {role} role.<br>" +
                $"You can register by clicking <a href='{link}'>here</a>." +
                $"<br><br><hr>Please do not reply to this message. This email is not monitored.");
        }
    }
}
