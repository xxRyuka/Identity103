using Microsoft.AspNetCore.Authorization;

namespace Mvc.Identity101.Requirements;

public class ExchangeExpireRequirement : IAuthorizationRequirement
{
}

public class ExchangeExpireHandler : AuthorizationHandler<ExchangeExpireRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExchangeExpireHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ExchangeExpireRequirement requirement)
    {
        
        
        if (context.User.IsInRole("god")) // eger kullanici god ise requirement'i basarili kiliyoruz
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        
        if (!context.User.HasClaim(x => x.Type == "ExchangeExpire")) // exchange expire yoksa pas geciyoz
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var ExchangeExpireClaim = context.User.Claims.FirstOrDefault(x => x.Type == "ExchangeExpire");

        if (DateTime.Now < Convert.ToDateTime(ExchangeExpireClaim.Value))
        {


            context.Fail();
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}