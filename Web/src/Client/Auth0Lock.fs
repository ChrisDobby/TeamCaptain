module Client.Auth0Lock

open Fable.Import.JS
open Fable.Core.JsInterop
open Fable.PowerPack
open Elmish.Browser.UrlParser

open Messages

let myConfig = 
    createObj [
       "auth" ==> createObj [
           "params" ==> createObj [
                "scope"    ==> "openid"
                "audience" ==> "https://chrisdobby.eu.auth0.com/api/v2/"
                "responseType" ==> "token id_token"
            ]
        ]
    ]

type Callback<'TErr,'TRes>  = System.Func<'TErr,'TRes,unit>

type [<AllowNullLiteral>] AuthResult =
    abstract accessToken : string with get

type [<AllowNullLiteral>] Profile = 
    abstract name        : string with get
    abstract user_id     : string with get
    abstract email       : string with get
    abstract picture     : string with get

let toUserProfile (auth:AuthResult) (profile:Profile) = {
        AccessToken = auth.accessToken
        Name = profile.name
        Email = profile.email
        Picture = profile.picture
        UserId = profile.user_id
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
                        if not <| isNull err then
                            GenericAuthException ("callbackException", (err |> Fable.Core.JsInterop.toJson))  |> reject
                        failwith "Both Result and Errors of callback are empty which should be impossible"
        ))
    l |> Fable.PowerPack.Promise.create

let auth0lock:JsConstructor<string,string,obj,Lock> = importDefault "auth0-lock/lib/index.js"

let auth0lockParser str : Parser<_,_> =
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
            if next = str then
                [ mkState (next :: visited) rest args value ]
            else
                []
    inner
