namespace StockMgmt.Core;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    private ApiResponse(bool success, string message, T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    // Factory method for successful responses (200 OK)
    public static ApiResponse<T> Ok(T? data = default, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>(true, message, data);
    }

    // Factory method for created responses (201 Created)
    public static ApiResponse<T> Created(T? data = default, string message = "Resource created successfully")
    {
        return new ApiResponse<T>(true, message, data);
    }

    // Factory method for failed responses (400/404/500 etc.)
    public static ApiResponse<T> Fail(string message, T? data = default)
    {
        return new ApiResponse<T>(false, message, data);
    }
}

// Non-generic version for responses without data
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    private ApiResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    // Factory method for successful responses (200 OK)
    public static ApiResponse Ok(string message = "Operation completed successfully")
    {
        return new ApiResponse(true, message);
    }

    // Factory method for created responses (201 Created)
    public static ApiResponse Created(string message = "Resource created successfully")
    {
        return new ApiResponse(true, message);
    }

    // Factory method for failed responses (400/404/500 etc.)
    public static ApiResponse Fail(string message)
    {
        return new ApiResponse(false, message);
    }
}

