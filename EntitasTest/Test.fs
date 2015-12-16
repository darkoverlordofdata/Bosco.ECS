module EntitasTest

open System    
open NUnit.Framework
open Entitas

let world = new World(16)

[<TestFixture>]
type FirstTest() =

    [<Test>]
    member this.TestIs1() =

        let e = world.CreateEntity()
        let id = e.Id
        Assert.AreEqual(id, 1)

