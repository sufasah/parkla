using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parkla.Business.Abstract;
using Parkla.Core.Options;

namespace Parkla.Business.Concrete;
public class MailService : IMailService
{
    private readonly ILogger _logger;
    private readonly string _email;
    private readonly string _password;
    public MailService(
        ILogger<MailService> logger,
        IOptions<SecretOptions> options
    ) {
        _logger = logger;
        _email = options.Value.ServerEmail;
        _password = options.Value.ServerEmailPassword;
    }

    public async Task<bool> SendMailAsync(
        MailMessage message, 
        CancellationToken cancellationToken = default
    ) {
        try {
            message.From = new MailAddress(_email, "Parkla");
            var encoding = new UTF8Encoding();
            message.BodyEncoding = encoding;
            message.HeadersEncoding = encoding;
            message.SubjectEncoding = encoding;

            using var smtpClient = new SmtpClient("smtp.office365.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_email, _password);
            await smtpClient.SendMailAsync(message, cancellationToken);
            return true;
        } catch(Exception e) {
            _logger.LogError(e, "Mail could not send");
            return false;
        }
    }
}