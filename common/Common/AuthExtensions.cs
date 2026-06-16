using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Common;

/// <summary>
/// Shared Keycloak authentication wiring so every service authenticates against
/// the same Aspire-provided Keycloak instance, realm and audience.
/// </summary>
public static class AuthExtensions
{
    /// <summary>The Aspire service name for the Keycloak server.</summary>
    public const string KeycloakServiceName = "keycloak";

    /// <summary>The Keycloak realm that issues the tokens.</summary>
    public const string Realm = "overflow";

    /// <summary>The expected audience claim on incoming tokens.</summary>
    public const string Audience = "overflow";

    /// <summary>
    /// Adds JWT bearer authentication backed by Keycloak plus authorization.
    /// </summary>
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddKeycloakJwtBearer(
                serviceName: KeycloakServiceName,
                realm: Realm,
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = Audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = new[] { 
                            $"http://{KeycloakServiceName}/realms/{Realm}",
                            "http://localhost:6001/realms/overflow", // For local development when "keycloak" doesn't resolve
                            "http://id.overflow.local/realms/overflow" // For local development with Aspire CLI's /etc/hosts modification
                             },
                    };
                });

        services.AddAuthorization();

        return services;
    }
}
