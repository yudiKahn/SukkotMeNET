
using IEsrog.Services.Email;

namespace IEsrog.Services;

public abstract record FireAndForgetData;
public record FireAndForgetSendEmailData(EmailType Subject, string Body, string To, string? Bcc = null) : FireAndForgetData;


public class FireAndForgetService
{
    readonly IServiceScopeFactory _ServiceFactory;

    public FireAndForgetService(IServiceScopeFactory serviceFactory)
    {
        _ServiceFactory = serviceFactory;
    }

    public async void Fire(FireAndForgetData data)
    {
        try
        {
            switch (data)
            {
                case FireAndForgetSendEmailData d1:
                {
                    using var scope = _ServiceFactory.CreateScope();
                    var service = scope.ServiceProvider.GetService<IEmailService>();
                    var res = await service!.SendAsync(d1.Subject, d1.Body, d1.To, d1.Bcc);
                }
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            
        }
    }
}