using BankAPI.Exceptions;

public static class ErrorHandlingMiddleware
{
    public static IApplicationBuilder UseCustomErrorHandler(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad request");
            }
            catch (UnauthorizedException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
            catch (ForbiddenException)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden");
            }
            catch (NotFoundException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Not found");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                Console.WriteLine(e.Message);
                await context.Response.WriteAsync("Internal server error");
            }
        });
        return applicationBuilder;
    }
}