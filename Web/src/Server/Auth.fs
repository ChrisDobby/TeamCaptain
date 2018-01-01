/// Login web part and functions for API web part request authorisation with JWT.
module Server.Auth

open Giraffe
open RequestErrors
open Microsoft.AspNetCore.Http

/// Invokes a function that produces the output for a web part if the HttpContext
/// contains a valid auth token. Use to authorise the expressions in your web part
/// code (e.g. WishList.getWishList).
let useToken next (ctx: HttpContext) f = task {
    match ctx.Request.Headers.TryGetValue "Authorization" with
    | true, accessToken ->
        match Seq.tryHead accessToken with
            Some token when token.StartsWith "Bearer " -> 
            let jwt = token.Replace("Bearer ","")
            match JsonWebToken.isValid jwt with
                | None -> return! FORBIDDEN "Accessing this API is not allowed" next ctx
                | Some token -> return! f token
            | _ -> return! BAD_REQUEST "Request doesn't contain a JSON Web Token" next ctx
    | _ -> return! BAD_REQUEST "Request doesn't contain a JSON Web Token" next ctx
}
