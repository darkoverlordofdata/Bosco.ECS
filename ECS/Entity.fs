namespace Bosco.ECS

open System
open System.Text
open System.Collections.Generic

(** 
 * Entity Events
 *)
type EntityReleasedArgs() =
  inherit System.EventArgs() 

type ComponentAddedArgs(index, newComponent) =
  inherit System.EventArgs()
  member this.index = index
  member this.newComponent = newComponent

type ComponentRemovedArgs(index, previous) =
  inherit System.EventArgs()
  member this.index = index
  member this.previous = previous

type ComponentReplacedArgs(index, previous, replacement) =
  inherit System.EventArgs()
  member this.index = index
  member this.previous = previous
  member this.replacement = replacement


type EntityReleasedDelegate = delegate of obj * EntityReleasedArgs -> unit
type ComponentAddedDelegate = delegate of obj * ComponentAddedArgs -> unit
type ComponentRemovedDelegate = delegate of obj * ComponentRemovedArgs -> unit
type ComponentReplacedDelegate = delegate of obj * ComponentReplacedArgs -> unit



(** 
 * Entity
 *)
[<AllowNullLiteral>]
type Entity (totalComponents:int) =

  let isNull x = match x with null -> true | _ -> false
  let notNull x = match x with null -> false | _ -> true

  (**
   * Component Active Pattern  
   * parse component class name
   *)
  let (|Component|) (s:string) =
      let s0 = s.Split(if s.IndexOf('+') = -1 then '.' else '+')
      let s1 = s0.[1]
      if s1.EndsWith("Component") then
          s1.Substring(0,s1.LastIndexOf("Component"))
      else
          s1

  (** parse component class name **)
  let parsec s =
      match s with 
      | Component (c) -> c

  let onComponentAdded                  = new Event<ComponentAddedDelegate, ComponentAddedArgs>()
  let onComponentRemoved                = new Event<ComponentRemovedDelegate, ComponentRemovedArgs>()
  let onComponentReplaced               = new Event<ComponentReplacedDelegate, ComponentReplacedArgs>()
  let onEntityReleased                  = new Event<EntityReleasedDelegate, EntityReleasedArgs>()
  let components: Component array       = (Array.zeroCreate totalComponents)
  let mutable componentsCache           = Array.empty<Component>
  let mutable toStringCache             = "" 

  member val OnComponentAdded           = onComponentAdded.Publish with get
  member val OnComponentRemoved         = onComponentRemoved.Publish with get
  member val OnComponentReplaced        = onComponentReplaced.Publish with get
  member val OnEntityReleased           = onEntityReleased.Publish with get

  member val internal refCount          = 0 with get, set                
  member val Id                         = 0 with get, set
  member val Name                       = "" with get, set
  member val IsEnabled                  = false with get, set
       
  member this.CHECK() =
    let mutable result = false
    for i = 0 to components.Length-1 do
      if not(isNull(components.[i])) then
        result <- true
    result

  member this.Initialize() =
      for i = 0 to components.Length-1 do
        components.[i] <- null
               
   (** 
   * AddComponent 
   *
   * @param index
   * @param component
   * @returns this entity
   *)
  member this.AddComponent(index:int, c:Component) =
    if not this.IsEnabled then 
      failwith "Entity is disabled, cannot add component"
    if this.HasComponent(index) then 
      failwithf "Entity already has component, cannot add at index %d, %s" index (this.ToString())

    components.[index] <- c
    componentsCache <- Array.empty<Component>
    toStringCache <- ""
    onComponentAdded.Trigger(this, new ComponentAddedArgs(index, c))
    this
  
  (** 
   * RemoveComponent 
   *
   * @param index
   * @returns this entity
   *)
  member this.RemoveComponent(index:int) =
    if not this.IsEnabled then 
      failwith "Entity is disabled, cannot remove component"
    if not(this.HasComponent(index)) then 
      failwithf "Entity does not have component, cannot remove at index %d, %s" index (this.ToString())

    this.replaceComponent(index, null)
    this

  (** 
   * ReplaceComponent 
   *
   * @param index
   * @param component
   * @returns this entity
   *)
  member this.ReplaceComponent(index:int, c:Component) =
    if not this.IsEnabled then 
      failwith "Entity is disabled, cannot replace at index %d, %s" index (this.ToString())

    if this.HasComponent(index) then
      this.replaceComponent(index, c)
    elif notNull(c) then
      this.AddComponent(index, c) |> ignore
    this

  (** 
   * GetComponent 
   *
   * @param index
   * @returns the component at index
   *)
  member this.GetComponent(index:int) =
    if not(this.HasComponent(index)) then 
      failwithf "Entity does not have component, cannot get at index %d, %s" index (this.ToString())

    components.[index]

  (** 
   * GetComponents 
   *
   * @returns a list of components
   *)
  member this.GetComponents() =
    if componentsCache.Length = 0 then
      //componentsCache <- Array.filter ((<>)null) components
      componentsCache <- Array.filter notNull components
    componentsCache

  (** 
   * HasComponent
   *
   * @param index
   * @returns true if entity has component at index
   *)
  member this.HasComponent(index:int) =
    notNull(components.[index])


  (** 
   * HasComponents
   *
   * @param indices array
   * @returns true if entity has all components in array
   *)
  member this.HasComponents(indices:int[]) =
    let mutable flag = true

    for index in indices do
      if isNull(components.[index]) then
        flag <- false
    flag

  (** 
   * HasAnyComponent
   *
   * @param indices array
   * @returns true if entity has any component in array
   *)
  member this.HasAnyComponent(indices:int[]) =
    let mutable flag = false

    for index in indices do
      if notNull(components.[index]) then
        flag <- true
    flag

  (** 
   * RemoveAllComponents
   *
   *)
  member this.RemoveAllComponents() =
    
    for i = 0 to components.Length-1 do
      if notNull(components.[i]) then
        this.replaceComponent(i, null)

  (** 
   * Retain (reference count)
   *
   *)
  member this.Retain() =
    this.refCount <- this.refCount + 1

  (** 
   * Release (reference count)
   *
   *)
  member this.Release() =
    this.refCount <- this.refCount - 1
    if this.refCount = 0 then
      //WHY??? - should't get triggered...
      onEntityReleased.Trigger(this, new EntityReleasedArgs())
    elif this.refCount < 0 then
      failwithf "Entity is already released %s" (this.ToString())

  (** 
   * ToString
   *
   *)
  override this.ToString() =
    if toStringCache = "" then

      let sb = new StringBuilder()
      sb.Append("Entity_") |> ignore
      sb.Append(this.Name) |> ignore
      sb.Append("(") |> ignore
      sb.Append(this.Id.ToString()) |> ignore
      sb.Append(")") |> ignore
      sb.Append("(") |> ignore
      let c = Array.filter notNull components
      for i = 0 to c.Length-1 do
        sb.Append(parsec(c.[i].GetType().ToString())) |> ignore
        if i < c.Length-1 then sb.Append(",") |> ignore
      sb.Append(")") |> ignore
      toStringCache <- sb.ToString()

    toStringCache

  (** 
   * destroy an entity
   *
   *)
  member this.destroy() =
    this.RemoveAllComponents()
    componentsCache <- Array.empty<Component>
    this.Name <- ""
    this.IsEnabled <- false

  (** 
   * replaceComponent 
   *
   * @param index
   * @param component
   *)
  member private this.replaceComponent(index, replacement) =
    let previousComponent = components.[index]
    if obj.ReferenceEquals(previousComponent, replacement) then
      onComponentReplaced.Trigger(this, new ComponentReplacedArgs(index, previousComponent, replacement))
    else
      components.[index] <- replacement
      componentsCache <- Array.empty<Component> 
      toStringCache <- ""
      if obj.ReferenceEquals(replacement, null) then
        onComponentRemoved.Trigger(this, new ComponentRemovedArgs(index, previousComponent))
      else
        onComponentReplaced.Trigger(this, new ComponentReplacedArgs(index, previousComponent, replacement))


type EntityEqualityComparer() =
  static member comparer with get() = new EntityEqualityComparer()
  interface IEqualityComparer<Entity> with

    member this.Equals(x, y) =
      if x.Id = y.Id then true else false

    member this.GetHashCode(e) =
      e.Id
    