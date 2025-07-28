// namespace Mvc.Identity101.CustomMiddlewares;
//
// public class StatusCodeCaptureMiddleware
// {
//     private readonly RequestDelegate _next;
//
//     public StatusCodeCaptureMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }
//
//     public async Task Invoke(HttpContext context)
//     {
//         await _next(context);
//
//         if (context.Response.StatusCode == 403)
//         {
//             // Loglama, özel yönlendirme, response değiştirme vs.
//             // Örnek: JSON API ise custom 403 response dön
//             if (context.Request.Path.StartsWithSegments("/api"))
//             {
//                 context.Response.ContentType = "application/json";
//                 await context.Response.WriteAsync("{\"error\": \"Forbidden\"}");
//             }
//         }
//     }
// }