namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open Entitas
open System
open System.Collections.Generic
open Microsoft.FSharp.Reflection

type SpriteRenderSystem(world) =
    interface IExecuteSystem with
        member this.Execute() =
            ()