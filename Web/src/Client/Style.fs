module Client.Style

open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Messages

module R = Fable.Helpers.React

let viewLink page description =
  R.a [ Style [ Padding "0 20px" ]
        Href (toHash page) ]
      [ R.str description]

let centerStyle direction =
    Style [ Display "flex"
            FlexDirection direction
            AlignItems "center"
            !!("justifyContent", "center")
            Padding "20px 0"
    ]

let words size message =
    R.span [ Style [ !!("fontSize", size |> sprintf "%dpx") ] ] [ R.str message ]

let buttonLink cssClass onClick elements = 
    R.a [ ClassName cssClass
          OnClick (fun _ -> onClick())
          OnTouchStart (fun _ -> onClick()) 
          Style [ !!("cursor", "pointer") ] ] elements

let floatingButton onClick iconName =
    R.a [ClassName "btn-floating btn-large waves-effect waves-light red"
         OnClick (fun _ -> onClick())
         OnTouchStart (fun _ -> onClick())]
         [
             R.i [ClassName "material-icons"] [str iconName]
         ]

let onEnter msg dispatch =
    OnKeyDown (fun (ev:React.KeyboardEvent) ->
        match ev with 
        | _ when ev.keyCode = 13. ->
            ev.preventDefault()
            dispatch msg
        | _ -> ())
