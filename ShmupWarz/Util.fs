namespace ShmupWarz
    
    open System
    open System.Collections.Generic
    open UnityEngine

    [<AbstractClass>]
    type Timer(delay, repeat) =

        let mutable acc = 0.0f
        let mutable fini = false
        let mutable stopped = false

        member this.Update() =
            this.Update(Time.deltaTime)

        member this.Update(delta) =
            if not(fini) && not(stopped) then
                acc <- acc + delta
            if acc >= delay then
                acc <- acc - delay
                if repeat then this.Reset() else fini <- true
                this.Execute()

        member this.Reset() =
            stopped <- false
            fini <- false
            acc <- 0.0f

        member this.IsDone() =
            fini

        member this.IsRunning() =
            not(fini) && acc < delay && not(stopped)

        member this.Stop() =
            stopped = true

        member this.SetDelay(value) =
            delay = value

        member this.GetPercentageRemaining() =
            if fini then 100
            elif stopped then 0
            else int (((1.0f-(delay-acc))/delay)*100.0f)

        member this.GetDelay() =
            delay

        abstract member Execute: unit -> unit
