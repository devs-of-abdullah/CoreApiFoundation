using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


public class UserOwnerOrAdminHandler : AuthorizationHandler<UserOwnerOrAdminRequirement, int>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnerOrAdminRequirement requirement, int enteredUserId)
    {

        if (context.User.IsInRole("admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }


        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);


        if (int.TryParse(userId, out int authenticatedStudentId) && authenticatedStudentId == enteredUserId)
        {
            context.Succeed(requirement);
        }


        return Task.CompletedTask;
    }
}