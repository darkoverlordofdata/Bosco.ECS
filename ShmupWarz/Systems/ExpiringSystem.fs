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

type ExpiringSystem(world:World) =

    let group = world.GetGroup(Matcher.AllOf(Matcher.Expires))

    interface IExecuteSystem with
        member this.Execute() =
            for e in (group.GetEntities()) do
                Debug.Log(sprintf "Check Expires: %f %s" (e.expires.delay) (e.ToString()))
                e.expires.delay <- e.expires.delay - Time.deltaTime
                if e.expires.delay <= 0.0f then
                    Debug.Log(sprintf "Expired: %s" (e.ToString()))
                    e.IsDestroy(true) |> ignore
