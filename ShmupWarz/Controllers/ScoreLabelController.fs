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

