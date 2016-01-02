namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
open UnityEngine.UI

type FpsController () =
    inherit MonoBehaviour ()

    [<DefaultValue>]
    val mutable label:Text
    let mutable totalFrames = 0
    let mutable fps = 0
    let mutable deltaTime = 0.0f
    let mutable elapsedTime = 0.0f

    member this.Start() = 
        this.label <- this.GetComponent():>Text

    member this.Update() =
        totalFrames <- totalFrames + 1
        elapsedTime <- elapsedTime + Time.deltaTime
        if elapsedTime > 1.0f then
            fps <- totalFrames
            totalFrames <- 0
            elapsedTime <- 0.0f

        this.label.text <- sprintf "fps %02d" fps
