namespace ShmupWarz
open System
open Bosco
open Bosco.Json
open Bosco.Utils
open UnityEngine
open UnityEngine.UI
open System.Collections
open System.Collections.Generic

type LeaderboardController () =
    inherit MonoBehaviour ()

    (** Display the leaderboard *)
    member this.Start() = 

        let MAX = 5
        try
            Properties.Init("shmupwarz", """[
                {""name"":""playSfx"", ""value"":true},
                {""name"":""playMusic"", ""value"":true}
            ]""")
        with
        | :? JsonSyntaxError as ex ->
            Debug.Log(sprintf "WARN: %s" ex.Message)

        let data = Properties.GetLeaderboard(MAX)


        for r=0 to MAX-1 do
            let col1 = GameObject.Find("UI Root (3D)/Camera/Anchor/Panel - Main/Window/Date"+r.ToString())
            let text1 = col1.GetComponent("UILabel"):?>UILabel

            let col2 = GameObject.Find("UI Root (3D)/Camera/Anchor/Panel - Main/Window/Score"+r.ToString())
            let text2 = col2.GetComponent("UILabel"):?>UILabel

            if r<data.Count then
                let row = JSON.Object(data.[r])
                let date = (row.["date"]).ToString()
                text1.text <- (sprintf "%s/%s/%s" (date.[4..5]) (date.[6..7]) (date.[2..3]) )
                text2.text <- (row.["score"]).ToString()
            else
                text1.text <- ""
                text2.text <- ""
