using System.Net.Mail;

namespace Parkla.Business.Abstract;
public interface IMailService
{
    public Task<bool> SendMailAsync(
        MailMessage message, 
        CancellationToken cancellationToken = default
    );
}