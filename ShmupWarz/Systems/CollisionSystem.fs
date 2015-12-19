namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open Entitas
open System
open System.Collections.Generic

type CollisionSystem(world) =
    interface IExecuteSystem with
        member this.Execute() =
            ()
    interface IInitializeSystem with
        member this.Initialize() =
            ()