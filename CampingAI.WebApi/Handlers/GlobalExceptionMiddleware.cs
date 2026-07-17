using System.Net;
using FluentValidation;

namespace CampingAI.WebApi.Handlers;
public class GlobalExceptionMiddleware {
    readonly RequestDelegate _next;
    readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext) {
        try {
            await _next(httpContext); // sigue con la pipeline
        } catch (ValidationException validationEx) {
            await HandleValidationExceptionAsync(httpContext, validationEx);
        } catch (KeyNotFoundException notFoundEx) {
            await HandleNotFoundExceptionAsync(httpContext, notFoundEx);
        } catch (Domain.Exceptions.DomainException domainEx) {
            await HandleDomainExceptionAsync(httpContext, domainEx);
        } catch (Exception ex) {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception) {
        _logger.LogError(exception, "Not controlled error captured in global error handler.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new Controllers.api.Shared.ErrorResponse("Se produjo un error inesperado en el servidor.");

        return context.Response.WriteAsJsonAsync(response);
    }


    private Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception) {
        _logger.LogWarning(exception, "Validation error captured in global error handler.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

        var errores = exception.Errors.Select(e => e.ErrorMessage).ToList();
        var response = new Controllers.api.Shared.ErrorResponse(errores, exception);

        return context.Response.WriteAsJsonAsync(response);
    }

    private Task HandleDomainExceptionAsync(HttpContext context, Domain.Exceptions.DomainException exception) {
        _logger.LogWarning(exception, "Domain error captured in global error handler.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var response = new Controllers.api.Shared.ErrorResponse($"Se produjo un error de validación en el servidor: {exception.Message}");

        return context.Response.WriteAsJsonAsync(response);
    }

    private Task HandleNotFoundExceptionAsync(HttpContext context, KeyNotFoundException exception) {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;

        var response = new Controllers.api.Shared.ErrorResponse($"No se ha encontrado el recurso: {exception.Message}");

        return context.Response.WriteAsJsonAsync(response);
    }
}