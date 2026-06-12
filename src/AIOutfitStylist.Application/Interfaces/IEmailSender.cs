namespace AIOutfitStylist.Application.Interfaces;

public interface IEmailSender
{
    Task<bool> SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
