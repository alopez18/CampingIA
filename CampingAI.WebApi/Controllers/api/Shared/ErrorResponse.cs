namespace CampingAI.WebApi.Controllers.api.Shared;
public class ErrorResponse : ResponseWrapper<string> {
    public ErrorResponse(List<string> errores, Exception ex) : base(errores) { }
    public ErrorResponse(string error, string? ex = null) : base(error, string.IsNullOrWhiteSpace(ex) ? string.Empty : ex) { }
}

public class ErrorResponse<T> : ResponseWrapper<T> {

    public ErrorResponse(List<string> errores) : base(errores) { }
    public ErrorResponse(string error) : base(error) { }
}