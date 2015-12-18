module EntitasTest

open System    
open NUnit.Framework
open Entitas
open Microsoft.FSharp.Reflection


type NameComponent() =
    inherit Component()
    member val name = "" with get, set

type PositionComponent() =
    inherit Component()
    member val x = 0 with get, set
    member val y = 0 with get, set
    member val z = 0 with get, set

type ComponentIds = 
    | NameComponent of NameComponent
    | PositionComponent of PositionComponent

let Components = FSharpType.GetUnionCases typeof<ComponentIds>

type MovementSystem() =
    interface IInitializeSystem with
        member this.Initialize() =
            ()
    interface IExecuteSystem with
        member this.Execute() =
            ()

[<TestFixture>]
type FirstTest() =

    let world = new World(2)

    let mutable e1:Entity = null
    let mutable e2:Entity = null
    let mutable e3:Entity = null

    [<Test>]//1st entity - no components
    member this.TestId1() =

        e1 <- world.CreateEntity()
        let id = e1.Id
        Assert.AreEqual(1, id)

    [<Test>]//2nd entity - name
    member this.TestId2() =

        e2 <- world.CreateEntity()
        e2.AddComponent(2, new NameComponent())
        Assert.AreEqual(e2.ToString(), "Entity_2(0)(Name)")


    [<Test>]//3rd entity - name, property
    member this.TestId3() =

        let mutable k = 0
        e3 <- world.CreateEntity()
        let s = e3.OnComponentAdded.Subscribe(fun o -> 
            k <- k+1
            if k = 1 then
                Assert.AreEqual("Entity_3(0)(Position)", e3.ToString())
            else
                Assert.AreEqual("Entity_3(0)(Position,Name)", e3.ToString())
        )
        e3.AddComponent(1, new PositionComponent())
        e3.AddComponent(2, new NameComponent())

    [<Test>]//check total counts
    member this.TestId4() =

        Assert.AreEqual(3, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

    [<Test>]//1 component matcher
    member this.TestId5() =

        let m1 = Matcher.AllOf(2)
        Assert.AreEqual(true, m1.Matches(e2))

        let m2 = Matcher.AllOf(1,2)
        Assert.AreEqual(false, m2.Matches(e2))

    [<Test>]//2 component matcher
    member this.TestId6() =

        let m1 = Matcher.AllOf(2)
        Assert.AreEqual(true, m1.Matches(e3))
        Assert.AreEqual("AllOf(2)", m1.ToString())

        let m2 = Matcher.AllOf(1,2)
        Assert.AreEqual(true, m2.Matches(e3))
        Assert.AreEqual("AllOf(1,2)", m2.ToString())


    [<Test>]//Groups
    member this.TestId7() =

        let g2 = world.GetGroup(Matcher.AllOf(2))
        let s2 = g2.GetEntities()
        Assert.AreEqual(2, s2.Length)

        let g = world.GetGroup(Matcher.AllOf(1,2))
        let s = g.GetEntities()
        Assert.AreEqual(1, s.Length)


    [<Test>]//Systems
    member this.TestId8() =

        let s = new MovementSystem()
        world.Add(s)
        world.Initialize()
        world.Execute()
