module Client.Auth0Lock

open Fable.Core.JsInterop
open Elmish.Browser.UrlParser

open Messages

let myConfig = 
    createObj [
       "auth" ==> createObj [
           "params" ==> createObj [
                "scope"    ==> "openid profile"
                "audience" ==> "https://chrisdobby.eu.auth0.com/userinfo"
                "responseType" ==> "token id_token"
            ]
        ]
    ]

type Callback<'TErr,'TRes>  = System.Func<'TErr,'TRes,unit>

type [<AllowNullLiteral>] AuthResult =
    abstract idToken : string with get

type [<AllowNullLiteral>] Profile = 
    abstract name        : string with get
    abstract sub         : string with get
    abstract picture     : string with get

let toUserProfile accessToken bearerToken (profile:Profile) = {
        AccessToken = accessToken
        BearerToken = bearerToken
        Name = profile.name
        Picture = profile.picture
        UserId = profile.sub
    }

type Lock = 
    abstract show        : unit -> unit
    abstract resumeAuth  : string -> Callback<obj,AuthResult> -> unit
    abstract getUserInfo : string -> Callback<obj,Profile> -> unit



//No idea what data they are returning with an error
exception GenericAuthException of string*string

let promisify<'res,'err when 'res : null and 'err : null> fn = 
    let l = fun resolve reject ->
        fn (Callback<'err,'res>(
                fun err res ->
                    if not <| isNull res then
                        resolve res
                    elif not <| isNull err then
                        GenericAuthException ("callbackException", (err |> Fable.Core.JsInterop.toJson))  |> reject
                    else
                        failwith "Both Result and Errors of callback are empty which should be impossible"
        ))
    l |> Fable.PowerPack.Promise.create

let auth0lock:JsConstructor<string,string,obj,Lock> = importDefault "auth0-lock/lib/index.js"

let auth0lockParser () : Parser<_,_> =
    let inner { visited = visited; unvisited = unvisited; args = args; value = value } =
        let mkState visited unvisited args value =
                { visited = visited
                  unvisited = unvisited
                  args = args
                  value = value }
        let parseAccessToken (l: string list) =
            let splitByToken (s: string) =
                [
                    s.Substring (0, 12)
                    s.Substring 13
                ]
            match l with
                | [] -> l
                | f::t -> 
                        if f.StartsWith "access_token=" then
                            List.append (splitByToken f) t
                        else
                            l

        match parseAccessToken unvisited with
        | [] -> []
        | next :: rest ->
            if next = "access_token" then
                [ mkState (next :: visited) rest args value ]
            else
                []
    inner

let auth0CallbackParser (str: string) = 
    let tokenLength = str.IndexOf "&expires_in"
    let expiresStart = tokenLength + 12
    let expiresLength = (str.IndexOf "&token_type") - expiresStart
    let bearerStart = (str.IndexOf "&id_token") + 10

    (
        str.Substring(0, tokenLength),
        int (str.Substring(expiresStart, expiresLength)),
        str.Substring bearerStart
    )