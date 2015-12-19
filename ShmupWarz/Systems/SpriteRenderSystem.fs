namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open Entitas
open System
open System.Collections.Generic

type SpriteRenderSystem(world) =
    interface IExecuteSystem with
        member this.Execute() =
            ()