namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
open UnityEngine.UI

(**
 * TextController
 *
 * Adapter to a Unity3D Text
 *)
[<AbstractClass>]
type TextController () =
    inherit MonoBehaviour ()
    (** Unity3D GameObject *)
    [<DefaultValue>] val mutable label:Text
    (** Return the default value *)
    abstract defaultValue : string with get
    (** initialize *)
    member this.Start() = 
        this.label <- this.GetComponent():>Text
        this.label.text <- this.defaultValue

(**
 * FpsController
 *
 * Attached to Assets/Scenes/Game Scene/Canvas/FPSText
 *)
type FpsController () =
    inherit TextController ()

    (**
     * calculate fps:
     *)
    let mutable totalFrames = 0
    let mutable fps = 0
    let mutable deltaTime = 0.0f
    let mutable elapsedTime = 0.0f

    override this.defaultValue with get() = sprintf "fps %02d" 0

    (** Update text value *)
    member this.Update() =
        totalFrames <- totalFrames + 1
        elapsedTime <- elapsedTime + Time.deltaTime
        if elapsedTime > 1.0f then
            fps <- totalFrames
            totalFrames <- 0
            elapsedTime <- 0.0f

        this.label.text <- sprintf "fps %02d" fps
