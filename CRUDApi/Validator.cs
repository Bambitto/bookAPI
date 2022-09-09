using FluentValidation;

namespace CRUDApi;

public static class Validator
{
    public static RouteHandlerBuilder WithValidator<T>(this RouteHandlerBuilder builder)
    {
        builder.Add(endpointBuilder =>
        {
            var oDelegate = endpointBuilder.RequestDelegate;
            endpointBuilder.RequestDelegate = async httpContext =>
            {
                var validator = httpContext.RequestServices.GetRequiredService<IValidator<T>>();

                httpContext.Request.EnableBuffering();

                var body = await httpContext.Request.ReadFromJsonAsync<T>();

                if (body is null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return;
                }

                var result = validator.Validate(body);

                if (!result.IsValid)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status411LengthRequired;
                    await httpContext.Response.WriteAsJsonAsync(result.Errors);
                    return;
                }

                httpContext.Request.Body.Position = 0;
                await oDelegate(httpContext);
            };
        });
        return builder;
    }
}
