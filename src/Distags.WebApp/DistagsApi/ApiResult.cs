using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Distags.WebApp.DistagsApi;

public class ApiResult
{
    public virtual bool IsSuccess { get; }

    public HttpStatusCode StatusCode { get; }

    public ProblemDetails? ProblemDetails { get; }

    protected ApiResult(HttpStatusCode statusCode)
    {
        IsSuccess = true;
        StatusCode = statusCode;
    }

    protected ApiResult(HttpStatusCode statusCode, ProblemDetails? problemDetails)
    {
        IsSuccess = false;
        StatusCode = statusCode;
        ProblemDetails = problemDetails;
    }

    public virtual void EnsureSuccess()
    {
        if (!IsSuccess)
            throw new ApiException(ProblemDetails);
    }

    public static ApiResult Success(HttpStatusCode statusCode)
    {
        return new ApiResult(statusCode);
    }

    public static ApiResult Failure(HttpStatusCode statusCode, ProblemDetails? problemDetails)
    {
        return new ApiResult(statusCode, problemDetails);
    }
}

public class ApiResult<TValue> : ApiResult
{
    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess => base.IsSuccess;

    public TValue? Value { get; }

    protected ApiResult(HttpStatusCode statusCode, TValue value)
        : base(statusCode)
    {
        Value = value;
    }

    protected ApiResult(HttpStatusCode statusCode, ProblemDetails? problemDetails)
        : base(statusCode, problemDetails)
    {
    }

    [MemberNotNull(nameof(Value))]
    public override void EnsureSuccess()
    {
        if (!IsSuccess || Value == null)
            throw new ApiException(ProblemDetails);
    }

    public static ApiResult<TValue> Success(HttpStatusCode statusCode, TValue response)
    {
        return new ApiResult<TValue>(statusCode, response);
    }

    public new static ApiResult<TValue> Failure(HttpStatusCode statusCode, ProblemDetails? problemDetails)
    {
        return new ApiResult<TValue>(statusCode, problemDetails);
    }
}
