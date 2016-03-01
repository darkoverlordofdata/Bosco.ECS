namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
open UnityEngine.UI
(**
 * ScoreLabelController
 *
 * Attached to Assets/Scenes/Game Scene/Canvas/TextScore
 *)
type ScoreLabelController () =
    inherit TextController ()

    let mutable score = 0
    let world = World.Instance

    override this.defaultValue with get() = sprintf "%05d" 0

    member this.Update() =
        if score <> world.score.value then
            score <- world.score.value
            this.label.text <- sprintf "%05d" score


(**
 * ActiveLabelController
 *
 * Attached to Assets/Scenes/Game Scene/Canvas/TextActive
 *)
type ActiveLabelController () =
    inherit TextController ()

    let mutable count = 0
    let world = World.Instance

    override this.defaultValue with get() = sprintf "Active: %05d" 0

    member this.Update() =
        if count <> world.count then
            count <- world.count
            this.label.text <- sprintf "Active: %05d" count

(**
 * ReuseLabelController
 *
 * Attached to Assets/Scenes/Game Scene/Canvas/TextReusable
 *)
type ReuseLabelController () =
    inherit TextController ()

    let mutable reuse = 0
    let world = World.Instance

    override this.defaultValue with get() = sprintf "Reuse: %05d" 0

    member this.Update() =
        if reuse <> world.reusableEntitiesCount then
            reuse <- world.reusableEntitiesCount
            this.label.text <- sprintf "Reuse: %05d" reuse

