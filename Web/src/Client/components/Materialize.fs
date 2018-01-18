module Client.Materialize

open Fable.Core

[<Emit("window['$'](document).ready(function() { window['$']('.timepicker').pickatime({ twelvehour: false }); })")>]
let InitialiseTimePickers () : unit = jsNative

[<Emit("window['$'](document).ready(function() { window['$']($1).material_select(($0))})")>]
let InitialiseSelect (changeHandler: unit -> unit) (selector: string) : unit = jsNative

[<Emit("window['$'](document).ready(function() { window['$']($1).off('change'); window['$']($1).on('change', ($0));})")>]
let AddChangeHandler (changeHandler: unit -> unit) (selector: string) : unit = jsNative