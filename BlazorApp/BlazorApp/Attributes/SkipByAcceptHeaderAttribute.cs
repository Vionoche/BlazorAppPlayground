using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace BlazorApp.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SkipByAcceptHeaderAttribute : Attribute, IActionConstraint
{
    public SkipByAcceptHeaderAttribute()
    {
        
    }

    public int Order => 0;
    
    public bool Accept(ActionConstraintContext context)
    {
        var requestAccept = context.RouteContext.HttpContext.Request.Headers[HeaderNames.Accept];
        
        if (StringValues.IsNullOrEmpty(requestAccept))
        {
            return true;
        }

        if (requestAccept.Count != 1)
        {
            return true;
        }

        var acceptMediaTypes = requestAccept[0]!.Split(',');
        if (acceptMediaTypes.Length != 1)
        {
            return true;
        }

        return acceptMediaTypes[0] != "*/*";
    }
}