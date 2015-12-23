namespace ShmupWarz

(**
 * Entitas Generated Systems for ShmupWarz
 *
 *)

open System
open System.Collections.Generic
open Entitas
open ShmupWarz
open UnityEngine

type SpawnEnemy1(world:World, delay, repeat) =
    inherit Timer(delay, repeat)
    override this.Execute() =
        world.CreateEnemy1() |> ignore

type SpawnEnemy2(world:World, delay, repeat) =
    inherit Timer(delay, repeat)
    override this.Execute() =
        world.CreateEnemy2() |> ignore

type SpawnEnemy3(world:World, delay, repeat) =
    inherit Timer(delay, repeat)
    override this.Execute() =
        world.CreateEnemy3() |> ignore



type EntitySpawningTimerSystem(world:World) =

    let timer1 = new SpawnEnemy1(world, 2.0f, true)
    let timer2 = new SpawnEnemy2(world, 6.0f, true)
    let timer3 = new SpawnEnemy3(world, 12.0f, true)

    interface IExecuteSystem with
        member this.Execute() =
            timer1.Update(Time.deltaTime)
            timer2.Update(Time.deltaTime)
            timer3.Update(Time.deltaTime)
