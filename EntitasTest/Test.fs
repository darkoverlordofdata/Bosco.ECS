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





let world = new World(3)
let sys = new Systems()


[<TestFixture>]
type FirstTest() =

    [<DefaultValue>] val mutable e1 : Entity
    [<DefaultValue>] val mutable e2 : Entity
    [<DefaultValue>] val mutable e3 : Entity

    [<Test>]
    member this.TestId1() =

        this.e1 <- world.CreateEntity()
        let id = this.e1.Id
        Assert.AreEqual(1, id)

    [<Test>]
    member this.TestId2() =

        this.e2 <- world.CreateEntity()
        //this.e2.AddComponent(1, new PositionComponent())
        this.e2.AddComponent(2, new NameComponent())
        Assert.AreEqual(this.e2.ToString(), "Entity_2(0)(Name)")


    [<Test>]
    member this.TestId3() =

        let mutable k = 0
        this.e3 <- world.CreateEntity()
        let s = this.e3.OnComponentAdded.Subscribe(fun o -> 
            k <- k+1
            if k = 1 then
                Assert.AreEqual("Entity_3(0)(Position)", this.e3.ToString())
            else
                Assert.AreEqual("Entity_3(0)(Position,Name)", this.e3.ToString())
        )
        this.e3.AddComponent(1, new PositionComponent())
        this.e3.AddComponent(2, new NameComponent())

    [<Test>]
    member this.TestId4() =

        Assert.AreEqual(3, world.count)
        Assert.AreEqual(0, world.retainedEntitiesCount)
        Assert.AreEqual(0, world.reusableEntitiesCount)

    [<Test>]
    member this.TestId5() =
        Assert.AreEqual("Entity_2(0)(Name)", this.e2.ToString())

        let m1 = Matcher.AllOf(2)
        Assert.AreEqual([|2|], m1.AllOfIndices)
        Assert.AreEqual(1, m1.AllOfIndices.Length)
        Assert.AreEqual(true, m1.Matches(this.e2))

        let m2 = Matcher.AllOf(1,2)
        Assert.AreEqual([|1;2|], m2.AllOfIndices)
        Assert.AreEqual(false, m2.Matches(this.e2))

    [<Test>]
    member this.TestId6() =
        Assert.AreEqual("Entity_3(0)(Position,Name)", this.e3.ToString())

        let m1 = Matcher.AllOf(2)
        Assert.AreEqual([|2|], m1.AllOfIndices)
        Assert.AreEqual(1, m1.AllOfIndices.Length)
        Assert.AreEqual(true, m1.Matches(this.e3))
        Assert.AreEqual("AllOf(2)", m1.ToString())

        let m2 = Matcher.AllOf(1,2)
        Assert.AreEqual([|1;2|], m2.AllOfIndices)
        Assert.AreEqual(true, m2.Matches(this.e3))

        Assert.AreEqual("AllOf(1,2)", m2.ToString())


    [<Test>]
    member this.TestId7() =

        Assert.AreEqual("Entity_1(0)()", this.e1.ToString())
        Assert.AreEqual("Entity_2(0)(Name)", this.e2.ToString())
        Assert.AreEqual("Entity_3(0)(Position,Name)", this.e3.ToString())

        let g2 = world.GetGroup(Matcher.AllOf(2))
        let s2 = g2.GetEntities()
        Assert.AreEqual(2, s2.Length)

        let g = world.GetGroup(Matcher.AllOf(1,2))
        let s = g.GetEntities()
        Assert.AreEqual(1, s.Length)



