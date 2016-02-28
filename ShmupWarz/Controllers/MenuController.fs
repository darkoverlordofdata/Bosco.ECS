namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
open UnityEngine.SceneManagement

type MenuController () =
    inherit MonoBehaviour ()

    member this.StartGame() =
        SceneManager.LoadScene("Game Scene")

    member this.LoadLeaderboard() =
        SceneManager.LoadScene("LeaderboardScene")

    member this.LoadCredits() =
        SceneManager.LoadScene("CreditsScene")

    member this.LoadMenu() =
        SceneManager.LoadScene("MenuScene")

