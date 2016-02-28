namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
open UnityEngine.UI

type ScoreLabelController () =
    inherit MonoBehaviour ()

    [<DefaultValue>]
    val mutable label:Text
    let mutable score = 0

    member this.Start() = 
        this.label <- this.GetComponent():>Text
        this.label.text <- sprintf "%05d" 0

    member this.Update() =
        if score <> World.Instance.score.value then
            score <- World.Instance.score.value
            this.label.text <- sprintf "%05d" score


type ActiveLabelController () =
    inherit MonoBehaviour ()

    [<DefaultValue>]
    val mutable label:Text
    let mutable count = 0

    member this.Start() = 
        this.label <- this.GetComponent():>Text
        this.label.text <- sprintf "Active: %05d" 0

    member this.Update() =
        if count <> World.Instance.count then
            count <- World.Instance.count
            this.label.text <- sprintf "Active: %05d" count

type ReuseLabelController () =
    inherit MonoBehaviour ()

    [<DefaultValue>]
    val mutable label:Text
    let mutable reuse = 0

    member this.Start() = 
        this.label <- this.GetComponent():>Text
        this.label.text <- sprintf "Reuse: %05d" 0

    member this.Update() =
        if reuse <> World.Instance.reusableEntitiesCount then
            reuse <- World.Instance.reusableEntitiesCount
            this.label.text <- sprintf "Reuse: %05d" reuse

type RetainLabelController () =
    inherit MonoBehaviour ()

    [<DefaultValue>]
    val mutable label:Text
    let mutable retain = 0

    member this.Start() = 
        this.label <- this.GetComponent():>Text
        this.label.text <- sprintf "Retain: %05d" 0

    member this.Update() =
        if retain <> World.Instance.retainedEntitiesCount then
            retain <- World.Instance.retainedEntitiesCount
            this.label.text <- sprintf "Retain: %05d" retain

