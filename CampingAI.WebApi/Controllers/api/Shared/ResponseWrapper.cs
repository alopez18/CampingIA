namespace CampingAI.WebApi.Controllers.api.Shared;
public abstract class ResponseWrapper<T> {
    public T? Data { get; set; }
    public List<string> Errores { get; set; }
    public bool Success { get; set; }


    //public ResponseWrapper() {
    //    this.Data = default;
    //    this.Errores = new List<string>();
    //    this.Success = true;
    //}
    public ResponseWrapper(T data) {
        this.Data = data;
        this.Errores = new List<string>();
        this.Success = true;
    }
    protected ResponseWrapper(List<string> errores) {
        this.Data = default;
        this.Errores = errores;
        this.Success = false;
    }
    protected ResponseWrapper(List<string> errores, T data) {
        this.Data = data;
        this.Errores = errores;
        this.Success = false;
    }

    protected ResponseWrapper(string error) {
        this.Data = default;
        this.Errores = new List<string> { error };
        this.Success = false;
    }

    protected ResponseWrapper(string error, T data) {
        this.Data = data;
        this.Errores = new List<string> { error };
        this.Success = false;
    }


    // Método de fábrica para errores
    public static ResponseWrapper<T> Error(string error) {
        return new ErrorResponse<T>(new List<string> { error });
    }

    public static ResponseWrapper<T> Error(List<string> errores) {
        return new ErrorResponse<T>(errores);
    }
}
