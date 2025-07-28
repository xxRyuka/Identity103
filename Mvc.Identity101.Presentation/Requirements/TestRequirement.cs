using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Mvc.Identity101.Requirements;

public class TestRequirement : IAuthorizationRequirement
{
    public int DynamicThresOld { get; set; }
}

public class TestRequirementHandler : AuthorizationHandler<TestRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TestRequirement requirement)
    {
        var mail1 = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        
        var mail = context.User.FindFirst(ClaimTypes.Email) ?? context.User.FindFirst(type:"email");
        
        if (mail is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        bool x = requirement.DynamicThresOld < mail.Value.Length;
        if (!x)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}