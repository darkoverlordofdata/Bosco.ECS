namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open Entitas
open System
open System.Collections.Generic

type ScaleAnimationSystem(world) =
    interface IExecuteSystem with
        member this.Execute() =
            ()