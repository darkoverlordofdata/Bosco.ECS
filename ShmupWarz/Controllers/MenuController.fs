namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine

type MenuController () =
    inherit MonoBehaviour ()

    member this.StartGame() =
        Application.LoadLevel("Game Scene")

    member this.LoadLeaderboard() =
        Application.LoadLevel("LeaderboardScene")

    member this.LoadCredits() =
        Application.LoadLevel("CreditsScene")

    member this.LoadMenu() =
        Application.LoadLevel("MenuScene")

