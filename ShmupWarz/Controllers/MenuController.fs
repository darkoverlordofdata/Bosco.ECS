namespace ShmupWarz
open System
open Bosco.ECS
open UnityEngine
open UnityEngine.SceneManagement

(**
 * MenuController
 *
 * The main menu switchboard for the application
 *
 * Attached to Assets/Scenes/MenuScene/UIManager
 * Attached to Assets/Scenes/LeaderboardScene/UIManager
 * Attached to Assets/Scenes/CreditsScene/UIManager
 * Attached to Assets/Scenes/Game Scene/UIManager
 *)

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

