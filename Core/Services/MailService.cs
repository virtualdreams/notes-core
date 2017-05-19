using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;

namespace notes.Core.Services
{
	public class MailService
	{
		private readonly IOptions<Settings> Options;
		private readonly ILogger<MailService> Log;

		public MailService(IOptions<Settings> options, ILogger<MailService> log)
		{
			Options = options;
			Log = log;
		}

		/// <summary>
		/// Send a reset password mail to the recipient.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="mail">The email address.</param>
		/// <param name="origin">The origin from where the user has the passswors reset requested.</param>
		/// <param name="token">The token to reset the password.</param>
		public void SendResetPasswordMail(string username, string mail, string origin, string token)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(Options.Value.Smtp.From));
			message.To.Add(new MailboxAddress(mail));
			message.Subject = $"[{Options.Value.SiteName}] - Reset Password";
			message.Body = new TextPart("plain")
			{
				Text =
$@"Hi {username},

You recently requested to reset your password for your {Options.Value.SiteName} account. Use the link below to reset it. This password reset is only valid for the next 1 hour.

{origin}/reset_password/{token}

If you did not request a password reset, please ignore this email or contact support if you have questions.

Thanks,
The {Options.Value.SiteName} Team

{Options.Value.SiteName} ({origin})"
			};

			SendMail(message);
		}

		/// <summary>
		/// Send a message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		private void SendMail(MimeMessage message)
		{
			if (Options.Value.Smtp.Enabled)
			{
				Log.LogInformation($"Send mail...");
				// send e-mail
				using (var client = new SmtpClient())
				{
					// accept all SSL certificates (in case the server supports STARTTLS)
					if (Options.Value.Smtp.SkipVerify)
						client.ServerCertificateValidationCallback = (s, c, h, e) => true;

					// connect to given host and port
					client.Connect(Options.Value.Smtp.Server, Options.Value.Smtp.Port, false);

					// disable authentication if username or password is empty
					if (!String.IsNullOrEmpty(Options.Value.Smtp.Username) && !String.IsNullOrEmpty(Options.Value.Smtp.Passwd))
						client.Authenticate(Options.Value.Smtp.Username, Options.Value.Smtp.Passwd);

					client.Send(message);
					client.Disconnect(true);
				}
			}
			else
			{
				Log.LogInformation($"Sending emails disabled.");
			}
		}
	}
}