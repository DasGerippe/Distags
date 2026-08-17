# Improvement: global exception handler for api

## Context
`Distags.Api/Program.cs` has no `app.UseExceptionHandler(...)` and no`app.UseDeveloperExceptionPage()` (not even behind a `IsDevelopment()` gate). Known, expected error cases already go through `ProblemDetailsResult.cs` and return clean, structured`ProblemDetails`. But a genuinely *unhandled* exception (NRE, EF Core exception, etc.) just falls through to the ASP.NET Core hosting default, which today happens to return an empty body with a 500 status.

That's currently "safe by accident" rather than by design - nothing stops someone from adding `UseDeveloperExceptionPage()` later (e.g. for local debugging convenience) without an environment guard, which would start leaking stack traces into response bodies. Those bodies already get read and logged verbatim by callers like `AutoTaggerRunner` (into local Serilog files), so anything the API leaks in an error response would also propagate into client-side logs.

## Problem

- No centralized handling of unhandled exceptions in `Distags.Api`.
- Unhandled-exception responses aren't shaped like the rest of the API's error contract (`ProblemDetails`), so clients can't rely on a consistent shape for all error responses.
- Unhandled exceptions aren't guaranteed to be logged server-side in a structured way (other projects in the solution use `SerilogUtils.ConfigureConsoleAndFileSinks`. not clear what the future could be - grafana stuff with prometheus/victoria, ELK stack, ...? 
- No explicit safeguard against accidentally leaking exception details (stack traces, messages that might include internal details) in Production if exception-detail middleware is ever added later without an environment check.

## Proposed solution

Add a global exception handler to api, e.g. via ASP.NET Core's e.g.:`AddExceptionHandler<T>()` and `app.UseExceptionHandler()`, that:
1. Logs the full exception (with stack trace) server-side via the same Serilog setup used elsewhere (`SerilogUtils`).
2. Returns a `ProblemDetails` response consistent with `ProblemDetailsResult.cs` conventions - generic message ("An unexpected error occurred") in all environments, no stack trace or exception message leaked to the client.
3. Optionally includes exception details in the response only when `app.Environment.IsDevelopment()` is true, for local debugging.

## Acceptance criteria

- Unhandled exceptions in `Distags.Api` return a structured `ProblemDetails` body instead of an empty/framework-default response.
- Unhandled exceptions are logged server-side with full details (stack trace included).
- No internal exception details are exposed to clients outside of `Development`.
- Behavior is covered by a quick manual check (e.g. temporarily throw from a controller action and confirm the response shape + logs).

## Related files

- `Distags.Api/Program.cs`
- `Distags.Api/ProblemDetailsResult.cs`
- `AutoTaggerRunner/DistagsApiClient.cs` (`EnsureSuccessAsync`) - root of all evil - keep your fingers crossed that comment is still there
