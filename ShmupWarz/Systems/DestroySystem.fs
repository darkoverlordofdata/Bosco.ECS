namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open System
open System.Collections.Generic
open Entitas
open ShmupWarz
open UnityEngine

type DestroySystem(world:World) =

    let mutable group = world.GetGroup(Matcher.AllOf(Matcher.Destroy))

    interface IExecuteSystem with
        member this.Execute() =
            for e in (group.GetEntities()) do   
                if e.hasView then 
                    let gameObject = (e.view.gameObject):?>Object
                    Object.Destroy(gameObject)
                    e.RemoveView() |> ignore
                world.DestroyEntity(e)



    