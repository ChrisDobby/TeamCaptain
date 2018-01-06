module Auth0Jwt

open Microsoft.IdentityModel.Tokens
open Microsoft.IdentityModel.Protocols
open Microsoft.IdentityModel.Protocols.OpenIdConnect
open System.IdentityModel.Tokens.Jwt
open System.Threading
open Giraffe
open Server
open ServerTypes

let private validationParameters = 
    let openIdConfig = task {
        let manager = ConfigurationManager<OpenIdConnectConfiguration>("https://chrisdobby.eu.auth0.com/.well-known/openid-configuration", OpenIdConnectConfigurationRetriever())
        return! manager.GetConfigurationAsync(CancellationToken.None)
    }

    let keys = openIdConfig.Result.SigningKeys

    TokenValidationParameters(
        ValidIssuer = "https://chrisdobby.eu.auth0.com/",
        ValidAudiences = ["1MD87NRRLv6doHbNL7FDUMckd0npGshr"],
        IssuerSigningKeys = keys)

let decode jwt =
    let handler = JwtSecurityTokenHandler()
    let _, token = handler.ValidateToken(jwt, validationParameters)
    let jwtToken = token :?> JwtSecurityToken
    { UserName = jwtToken.Subject }

let isValid jwt =
    try
        let rights = decode jwt
        Some rights
    with
    | _ -> None