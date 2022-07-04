using System.Text;

public class TestMiddleware
{
    private readonly RequestDelegate _next;

    public TestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var bufferSize = 1024 * 100;
        context.Request.EnableBuffering(bufferSize);

        // Leave the body open so the next middleware can read it.
        using (var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: bufferSize,
            leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            // Do some processing with body…

            // Reset the request body stream position so the next middleware can read it
            context.Request.Body.Position = 0;
        }

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}