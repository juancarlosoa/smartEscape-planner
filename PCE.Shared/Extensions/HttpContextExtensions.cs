using Microsoft.AspNetCore.Mvc;

namespace PCE.Shared.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Obtiene el UserSlug del header X-User-Slug inyectado por el Gateway.
    /// El Gateway es responsable de validar OAuth2/JWT y extraer el userSlug.
    /// </summary>
    public static string GetUserSlug(this ControllerBase controller)
    {
        if (controller.Request.Headers.TryGetValue("X-User-Slug", out var headerValue) 
            && !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        throw new UnauthorizedAccessException("X-User-Slug header missing. Request must pass through Gateway.");
    }
}
