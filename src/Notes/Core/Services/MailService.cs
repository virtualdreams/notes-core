using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Notes.Core.Interfaces;
using Notes.Options;
using System.Threading.Tasks;
using System;

namespace Notes.Core.Services
{
	public class MailService : IMailService
	{
		private readonly ILogger<MailService> Log;

		private readonly AppSettings AppSettings;

		private readonly MailSettings MailSettings;

		public MailService(
			ILogger<MailService> log,
			IOptionsSnapshot<AppSettings> settings,
			IOptionsSnapshot<MailSettings> mail)
		{
			Log = log;
			AppSettings = settings.Value;
			MailSettings = mail.Value;
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
			message.From.Add(MailboxAddress.Parse(MailSettings.MailFrom));
			message.To.Add(MailboxAddress.Parse(mail));
			message.Subject = $"[{AppSettings.SiteName}] - Reset Password";
			message.Body = new TextPart("plain")
			{
				Text =
$@"Hi {username},

You recently requested to reset your password for your ""{AppSettings.SiteName}"" account. Use the link below to reset it. This password reset is only valid for the next 1 hour.

{origin}/reset_password/{token}

If you did not request a password reset, please ignore this email or contact support if you have questions.

Thanks,
The {AppSettings.SiteName} Team

{AppSettings.SiteName} ({origin})"
			};

			await SendMailAsync(message);
		}

		/// <summary>
		/// Send a message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		private async Task SendMailAsync(MimeMessage message)
		{
			if (MailSettings.Enabled)
			{
				Log.LogInformation($"Send mail...");
				using (var client = new SmtpClient())
				{
					// accept all SSL certificates (in case the server supports STARTTLS)
					if (MailSettings.DisableCertificateValidation)
					{
						Log.LogInformation($"Verify certificate disabled.");
						client.ServerCertificateValidationCallback = (s, c, h, e) => true;
					}

					// disable check certificate revocation 
					if (MailSettings.DisableCheckCertificateRevocation)
					{
						Log.LogInformation($"Check certificate revocation disabled.");
						client.CheckCertificateRevocation = false;
					}

					// connect to given host and port
					client.Connect(MailSettings.Host, MailSettings.Port);

					// disable authentication if username or password is empty
					if (!String.IsNullOrEmpty(MailSettings.Username) && !String.IsNullOrEmpty(MailSettings.Password))
					{
						Log.LogInformation($"Set authentication credentials.");
						client.Authenticate(MailSettings.Username, MailSettings.Password);
					}

					await client.SendAsync(message);
					await client.DisconnectAsync(true);
					Log.LogInformation($"Mail sent.");
				}
			}
			else
			{
				Log.LogInformation($"Sending emails disabled.");
			}
		}
	}
}