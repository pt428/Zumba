using Humanizer;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using QRCoder;
using System.Runtime.Versioning;
using Zumba.Models;

namespace Zumba.Services
{
    public class EmailService
    {
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}
        //******************************************************************
        //***************** GENERATE QR CODE   *****************************
        //******************************************************************
		[SupportedOSPlatform("windows")]
		public byte[] GenerateQrCode(string content )
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);

                using (var qrCodeImage = qrCode.GetGraphic(20))
                using (var ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
         //******************************************************************
        //***************** CREATE EMAIL WITH QR CODE  *****************************
        //******************************************************************
        public MimeMessage CreateEmailWithQrCode(List<string> to, string subject, string text, string qrCodeBase64, byte[] qrcode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            foreach (var item in to)
            {
                message.To.Add(new MailboxAddress("", item));
            }
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.Attachments.Add("qrcode.png", qrcode, new ContentType("image", "png"));
            bodyBuilder.HtmlBody = $@"<p>{text}</p><img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' style='width:100px;'/>";
            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }
         //******************************************************************
        //***************** SEND EMAIL   *****************************
        //******************************************************************
		[SupportedOSPlatform("windows")]
		public async Task<bool> SendEmailAsync(List<string>  toEmail, string subject, string text, string qrCodeText)
        {
            bool result =false;
			// Generate QR code
			var qrCodeImage = GenerateQrCode(qrCodeText);
			// Convert QR code image to base64 string
			string qrCodeBase64 = Convert.ToBase64String(qrCodeImage);
		 
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            foreach (var email in toEmail)
            {
                if (email != "") { message.To.Add(new MailboxAddress("", email)); }
               
            }
			message.Subject = subject;

			var bodyBuilder = new BodyBuilder();
            if (qrCodeText != string.Empty)
            {
                bodyBuilder.Attachments.Add("qrcode.png", qrCodeImage, new ContentType("image", "png"));
                bodyBuilder.HtmlBody = $@"<p>{text}</p><img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' style='width:100px;'/>";
            }
            else
            {
				bodyBuilder.HtmlBody = $@"<p>{text}</p>";
            }
			message.Body = bodyBuilder.ToMessageBody();
			using (var smtp = new SmtpClient())
            {
				try
				{
					// Ignore SSL certificate validation
					smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSettings.SenderEmail, _emailSettings.Password);
                await smtp.SendAsync(message);
                smtp.Disconnect(true);
                    result = true;
				}
				catch (Exception  )
				{
					 
				}

			}
            return result;
        }
 

    }

}
