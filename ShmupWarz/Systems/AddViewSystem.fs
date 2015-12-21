namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open Entitas
open System
open System.Collections.Generic
open UnityEngine

type AddViewSystem(world:World) =

    let _viewContainer = ((new GameObject("Views")).transform)

    interface IInitializeSystem with
        member this.Initialize() =

            world.GetGroup(Matcher.Resource).OnEntityAdded.AddHandler(fun sender args ->

                let e = args.entity
                let res = Resources.Load<GameObject>(e.resource.name)
                let gameObject:GameObject = UnityEngine.Object.Instantiate(res):?>GameObject

                if not(IsNull(gameObject)) then
                    if e.hasPosition then
                        let pos = e.position
                        gameObject.transform.position <- new Vector3(pos.x, pos.y, 0.0f)

                    if e.hasScale then
                        let scale = e.scale
                        gameObject.transform.localScale <- new Vector3(scale.x, scale.y, 0.0f)

                    gameObject.transform.SetParent(_viewContainer, false)
                    e.AddView(gameObject) |> ignore
            )

