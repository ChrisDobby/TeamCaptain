module Client.Materialize

open Fable.Core

[<Emit("window['$'](document).ready(function() { window['$']('select').material_select(); })")>]
let InitialiseSelects () : unit = jsNative

[<Emit("window['$'](document).ready(function() { window['$']('.timepicker').pickatime({ twelvehour: false }); })")>]
let InitialiseTimePickers () : unit = jsNative
