using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDeviceManager.Database;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Lib.Extensions;

public class ApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApplicationDbContext _database;
    public ApiMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        IServiceScope scope = serviceScopeFactory.CreateScope();
        _database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        string apikey = context.Request.Headers[ApiConstants.ApiKey]!;

        //do the checking
        if (_database.Users.All(x => x.ApiKey != apikey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; 
            await context.Response.WriteAsync("Access denied!");
            return;
        }

        //pass request further if correct
        await _next(context);
    }
}
public static class ApiMiddlewareExtensions
{
    public static IApplicationBuilder UseApiRequestValidation(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiMiddleware>();
    }
}