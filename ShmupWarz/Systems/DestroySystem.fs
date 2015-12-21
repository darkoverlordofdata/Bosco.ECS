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

    let group = world.GetGroup(Matcher.AllOf(Matcher.Destroy))

    interface IInitializeSystem with
        member this.Initialize() =
            world.GetGroup(Matcher.View).OnEntityRemoved.AddHandler(fun sender args ->
            
                let newComponent = (args.newComponent):?>ViewComponent
                let gameObject = (newComponent.gameObject):?>Object
                Object.Destroy(gameObject)
                Debug.Log(sprintf "DestroyEntity %s" (args.entity.ToString()))
                world.DestroyEntity(args.entity)
            )

    interface IExecuteSystem with
        member this.Execute() =
            for e in (group.GetEntities()) do   
                Debug.Log(sprintf "RemoveView %s" (e.ToString()))
                e.RemoveView() |> ignore
                    


    