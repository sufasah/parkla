using System.Net.Mail;

namespace Parkla.Business.Abstract;
public interface IMailService
{
    public Task SendMailAsync(
        MailMessage message, 
        CancellationToken cancellationToken = default
    );
}