using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using System;
using notes.Core.Interfaces;

namespace notes.Core.Services
{
	public class MailService : IMailService
	{
		private readonly Settings Options;
		private readonly ILogger<MailService> Log;

		public MailService(IOptionsSnapshot<Settings> settings, ILogger<MailService> log)
		{
			Options = settings.Value;
			Log = log;
		}

		/// <summary>
		/// Send a reset password mail to the recipient.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="mail">The email address.</param>
		/// <param name="origin">The origin from where the user has the passswors reset requested.</param>
		/// <param name="token">The token to reset the password.</param>
		public async Task SendResetPasswordMailAsync(string username, string mail, string origin, string token)
		{
			var message = new MimeMessage();
			message.From.Add(MailboxAddress.Parse(Options.Smtp.From));
			message.To.Add(MailboxAddress.Parse(mail));
			message.Subject = $"[{Options.SiteName}] - Reset Password";
			message.Body = new TextPart("plain")
			{
				Text =
$@"Hi {username},

You recently requested to reset your password for your ""{Options.SiteName}"" account. Use the link below to reset it. This password reset is only valid for the next 1 hour.

{origin}/reset_password/{token}

If you did not request a password reset, please ignore this email or contact support if you have questions.

Thanks,
The {Options.SiteName} Team

{Options.SiteName} ({origin})"
			};

			await SendMailAsync(message);
		}

		/// <summary>
		/// Send a message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		private async Task SendMailAsync(MimeMessage message)
		{
			if (Options.Smtp.Enabled)
			{
				Log.LogInformation($"Send mail...");
				// send e-mail
				using (var client = new SmtpClient())
				{
					// accept all SSL certificates (in case the server supports STARTTLS)
					if (Options.Smtp.SkipVerify)
						client.ServerCertificateValidationCallback = (s, c, h, e) => true;

					// connect to given host and port
					client.Connect(Options.Smtp.Server, Options.Smtp.Port, false);

					// disable authentication if username or password is empty
					if (!String.IsNullOrEmpty(Options.Smtp.Username) && !String.IsNullOrEmpty(Options.Smtp.Passwd))
						client.Authenticate(Options.Smtp.Username, Options.Smtp.Passwd);

					await client.SendAsync(message);
					await client.DisconnectAsync(true);
				}
			}
			else
			{
				Log.LogInformation($"Sending emails disabled.");
			}
		}
	}
}