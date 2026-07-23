using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public static ApiResponse<T> Ok(T data, string message = "Success") => new()
        {
            Success = true,
            Message = message,
            Data = data
        };
        public static ApiResponse<T> Fail(string error) => new()
        {
            Success = false,
            Message = error,
            Errors = [error]
        };
        public static ApiResponse<T> Fail(List<string> errors) => new()
        {
            Success = false,
            Message = "One or more validation errors occured.",
            Errors = errors
        };
    }
}
