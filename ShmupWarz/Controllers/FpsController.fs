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


    //[<DefaultValue>]
    //val mutable private data:JSONArray

    member this.Start() = 

        let MAX = 5
        Properties.Init("shmupwarz", """[
            {""name"":""playSfx"", ""value"":true},
            {""name"":""playMusic"", ""value"":true}
        ]""")
        let data = Properties.GetLeaderboard(MAX)


        for r=0 to MAX-1 do
            let mutable score = ""
            let mutable yyyymmdd = ""

            if r<data.Count then
                let row = JSON.Object(data.[r])
                score <- Convert.ToString(row.["score"])
                yyyymmdd <- Convert.ToString(row.["date"])

            let col1 = GameObject.Find("Canvas/Panel/TextRow"+(r+1).ToString()+"Date")
            let text1 = col1.GetComponent("Text"):?>Text
            text1.text <- yyyymmdd

            let col2 = GameObject.Find("Canvas/Panel/TextRow"+(r+1).ToString()+"Score")
            let text2 = col2.GetComponent("Text"):?>Text
            text2.text <- score