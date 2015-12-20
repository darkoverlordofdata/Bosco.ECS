namespace ShmupWarz
(**
 * Entity Factory
 *
 *)
[<AutoOpen>]
module EntityFactory =

    open Entitas
    open System
    open System.Collections.Generic
    open UnityEngine

    let rnd = new System.Random()

    let isNull x = match x with null -> true | _ -> false
    let notNull x = match x with null -> false | _ -> true

    type World with

        member this.CreatePlayer() =
            let pos = Vector3(80.0f, 100.0f, 0.0f)
            this.CreateEntity()
                .AddBounds(1.0f)
                .AddHealth(100.0f, 100.0f)
                .AddPosition(pos.x, pos.y, pos.z)
                .AddVelocity(0.0f, 0.0f, 0.0f)
                .IsPlayer(true)
                .AddResource("Fighter")

