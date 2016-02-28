(*
 * Bosco ECS
 *)
namespace Bosco.ECS

open System
open System.Text
open System.Collections.Generic

(**
  * Base Component Type
  *)
[<AbstractClass>][<AllowNullLiteral>]
type Component() = class end
    //static member None with get() = 0

and IOwner = 
  abstract member onEntityReleased: Entity -> unit
  abstract member onEntityChanged: Entity * int * Component -> unit


(**
  * Interface: System with an Initialization phase
  * Initialize is called before the game loop is started
  *)
and IInitializeSystem =
  abstract member Initialize: unit -> unit

(**
  * Interface: System with an Execute
  * Execute is called once per game loop
  *)
and IExecuteSystem =
  abstract member Execute: unit -> unit

and Entity (totalComponents:int, owner:IOwner) =

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

  let components: Component array       = (Array.zeroCreate totalComponents)
  let mutable componentsCache           = Array.empty<Component>
  let mutable toStringCache             = "" 

  new (owner:IOwner) = Entity(0, owner)

  member val internal refCount          = 0 with get, set                
  member val Id                         = 0 with get, set
  member val Name                       = "" with get, set
  member val IsEnabled                  = false with get, set
       
  member this.IsNull() =
    if totalComponents = 0 then
      true
    else
      false

  member this.Initialize(name:string, id:int) =
    for i = 0 to components.Length-1 do
    components.[i] <- null
    this.IsEnabled <- true
    this.Id <- id
    this.Name <- name
         
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
    owner.onEntityChanged(this, index, c)
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
      owner.onEntityReleased(this)
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
    this.IsEnabled <- false

  (** 
   * replaceComponent 
   *
   * @param index
   * @param component
   *)
  member private this.replaceComponent(index, replacement) =
    let previousComponent = components.[index]
    if not(obj.ReferenceEquals(previousComponent, replacement)) then
      components.[index] <- replacement
      componentsCache <- Array.empty<Component> 
      toStringCache <- ""
      if obj.ReferenceEquals(replacement, null) then
        owner.onEntityChanged(this, index, previousComponent)


and EntityEqualityComparer() =
  static member comparer with get() = new EntityEqualityComparer()
  interface IEqualityComparer<Entity> with

    member this.Equals(x, y) =
      if x.Id = y.Id then true else false

    member this.GetHashCode(e) =
      e.Id

and Matcher () =
  static let mutable uniqueId           = 0
  do uniqueId <- uniqueId+1

  let _id                               = uniqueId
  let mutable _indices                  = Array.empty
  let mutable _allOfIndices             = Array.empty
  let mutable _anyOfIndices             = Array.empty
  let mutable _noneOfIndices            = Array.empty
  let mutable toStringCache             = ""

  static let toStringHelper(sb:StringBuilder, text:string, indices:int[]) =
    if indices.Length > 0 then
      sb.Append(text+"(") |> ignore
      for i=0 to indices.Length-1 do
        sb.Append(indices.[i].ToString()) |> ignore
        if i < indices.Length-1 then sb.Append(",") |> ignore
      sb.Append(")") |> ignore

  member val uuid = System.Guid.NewGuid().ToString() with get

  member this.Id
    with get() = _id
  member this.Indices
    with get():int[] = 
      if _indices.Length = 0 then
        _indices <- this.mergeIndices()
      _indices
  member internal this.AllOfIndices
    with get() = _allOfIndices
    and  set(value) = _allOfIndices <- value
  member internal this.AnyOfIndices
    with get() = _anyOfIndices
    and  set(value) = _anyOfIndices <- value
  member internal this.NoneOfIndices
    with get() = _noneOfIndices
    and  set(value) = _noneOfIndices <- value

  (** 
   * AnyOf 
   *
   * @param indices
   * @returns this 
   *)
  member this.AnyOf([<ParamArray>] indices: int[]) =
    _anyOfIndices <- Matcher.distinctIndices(_indices)
    _indices <- Array.empty
    this

  member this.AnyOf([<ParamArray>] matchers: Matcher[]) =
    this.AnyOf(Matcher.mergeIndices(matchers))

  (** 
   * NoneOf 
   *
   * @param indices
   * @returns this 
   *)
  member this.NoneOf([<ParamArray>] indices: int[]) =
    _noneOfIndices <- Matcher.distinctIndices(_indices)
    _indices <- Array.empty
    this

  member this.NoneOf([<ParamArray>] matchers: Matcher[]) =
    this.NoneOf(Matcher.mergeIndices(matchers))
   
  member this.mergeIndices():int[] =
    let indicesList = new ResizeArray<int>()
    indicesList.AddRange(_allOfIndices)
    indicesList.AddRange(_anyOfIndices)
    indicesList.AddRange(_noneOfIndices)
    _indices <- Matcher.distinctIndices(indicesList.ToArray())
    _indices

  (** 
   * Matches 
   *
   * @param entity
   * @returns true if entity is a match 
   *)
  member this.Matches(entity:Entity) =
    let matchesAllOf = if _allOfIndices.Length = 0 then true else entity.HasComponents(_allOfIndices)
    let matchesAnyOf = if _anyOfIndices.Length = 0 then true else entity.HasAnyComponent(_allOfIndices)
    let matchesNoneOf = if _noneOfIndices.Length = 0 then true else not(entity.HasAnyComponent(_allOfIndices))
    matchesAllOf && matchesAnyOf && matchesNoneOf



  (** 
   * AllOf 
   *
   * @param indices
   * @returns this 
   *)
  static member AllOf([<ParamArray>] indices: int[]) =
    let matcher = new Matcher()
    matcher.AllOfIndices <- Matcher.distinctIndices(indices)
    matcher

  static member AllOf([<ParamArray>] matchers: Matcher[]) =
    Matcher.AllOf(Matcher.mergeIndices(matchers))

  (** 
   * AnyOf 
   *
   * @param indices
   * @returns this 
   *)
  static member AnyOf([<ParamArray>] indices: int[]) =
    let matcher = new Matcher()
    matcher.AnyOfIndices <- Matcher.distinctIndices(indices)
    matcher

  static member AnyOf([<ParamArray>] matchers: Matcher[]) =
    Matcher.AnyOf(Matcher.mergeIndices(matchers))

  (** 
   * mergeIndicse 
   *
   * @param matchers
   * @returns array of indices for the matchers
   *)
  static member mergeIndices(matchers:Matcher[]):int[] =
    let mutable indices = (Array.zeroCreate matchers.Length)
    for i=0 to matchers.Length-1 do
      let matcher = matchers.[i]
      if matcher.Indices.Length <> 1 then
        failwithf "Matcher indices length not = 1 %s" (matchers.[i].ToString())
      indices.[i] <- matcher.Indices.[0]
    indices

  (** 
   * distinctIndicse 
   *
   * @param indices
   * @returns array of indices with duplicates removed
   *)
  static member distinctIndices(indices:int[]):int[] = 
    let indicesSet = new HashSet<int>(indices)
    let mutable uniqueIndices = (Array.zeroCreate indicesSet.Count)
    indicesSet.CopyTo(uniqueIndices)
    Array.Sort(uniqueIndices)
    uniqueIndices

  (** 
   * ToString
   *
   * @returns the string representation of this matcher
   *)
  override this.ToString() =


    if toStringCache = "" then

      let sb = new StringBuilder()
      toStringHelper(sb, "AllOf", _allOfIndices)
      toStringHelper(sb, "AnyOf", _anyOfIndices)
      toStringHelper(sb, "NoneOf", _noneOfIndices)
      toStringCache <- sb.ToString()

    toStringCache


(** 
 * Group Events
 *)
and GroupChangedArgs(entity, index, newComponent) =
  inherit System.EventArgs()
  member this.entity = entity
  member this.index = index
  member this.newComponent = newComponent

and GroupUpdatedArgs(entity, index, prevComponent, newComponent) =
  inherit System.EventArgs()
  member this.entity = entity
  member this.index = index
  member this.prevComponent = prevComponent
  member this.newComponent = newComponent

and GroupChangedDelegate = delegate of obj * GroupChangedArgs -> unit

(** 
 * Group
 *)
and Group (matcher:Matcher) =
 
  [<DefaultValue>] val mutable singleEntityCache:Entity
  [<DefaultValue>] val mutable singleEntityCacheFlag:bool
  let isNull x = match x with null -> true | _ -> false

  let onEntityAdded                     = new Event<GroupChangedDelegate, GroupChangedArgs>()
  let onEntityRemoved                   = new Event<GroupChangedDelegate, GroupChangedArgs>()
  let entities:HashSet<Entity>          = new HashSet<Entity>(EntityEqualityComparer.comparer)
  let mutable entitiesCache             = Array.empty<Entity>
  let mutable toStringCache             = ""

  member val OnEntityAdded              = onEntityAdded.Publish with get
  member val OnEntityRemoved            = onEntityRemoved.Publish with get
  member val matcher                    = matcher with get

  member this.count                     
    with get() = entities.Count

  (** 
   * HandleEntitySilently
   *
   * @param entity
   *)
  member this.HandleEntitySilently(entity) =
    if matcher.Matches(entity) then
      this.addEntitySilently(entity)
    else
      this.removeEntitySilently(entity)

  (** 
   * HandleEntity
   *
   * @param entity
   * @param index
   * @param component
   *)
  member this.HandleEntity(entity, index, comp) =
    if matcher.Matches(entity) then
      this.addEntity(entity, index, comp)
    else
      this.removeEntity(entity, index, comp)

  (** 
   * addEntitySilently
   *
   * @param entity
   *)
  member this.addEntitySilently(entity) =
    let added  = entities.Add(entity)
    if added then
      entitiesCache <- Array.empty<Entity>
      this.singleEntityCacheFlag <- false
      entity.Retain()
    added

  (** 
   * removeEntitySilently
   *
   * @param entity
   *)
  member this.removeEntitySilently(entity) =
    let removed = entities.Remove(entity)
    if removed then
      entitiesCache <- Array.empty<Entity>
      this.singleEntityCacheFlag <- false
      //entity.Retain()
      entity.Release()
    removed

  //member this.addEntity(entity) =
  //  match addEntitySilently(entity) with
  //  | true -> this.OnEntityAdded
  //  | _ -> null

  member this.addEntity(entity, index, comp) =
    if this.addEntitySilently(entity) then
      onEntityAdded.Trigger(this, new GroupChangedArgs(entity, index, comp))

  (** 
   * removeEntity
   *
   * @param entity
   * @param index
   * @param component
   *)
  member this.removeEntity(entity, index, comp) =
    if this.removeEntitySilently(entity) then
      onEntityRemoved.Trigger(this, new GroupChangedArgs(entity, index, comp))

  (** 
   * ContainsEntity
   *
   * @param entity
   * @returns true if the group has the entity
   *)
  member this.ContainsEntity(entity) =
    entities.Contains(entity)
  
  (** 
   * GetSingleEntity
   *
   * @returns entity or null
   *)
  member this.GetSingleEntity() =
    if not(this.singleEntityCacheFlag) then
      match entities.Count with
      | 1 ->
        use mutable enumerator = entities.GetEnumerator()
        enumerator.MoveNext() |> ignore
        this.singleEntityCache <- enumerator.Current
        this.singleEntityCacheFlag <- true
      | 0 ->
        this.singleEntityCache <- World.NullEntity
        this.singleEntityCacheFlag <- false
      | _ ->
        failwithf "Single Entity Execption %s" (matcher.ToString())
    this.singleEntityCache
  
  (** 
   * GetEntities
   *
   * @returns the array of entities
   *)
  member this.GetEntities() =
    if entitiesCache.Length = 0 then
      entitiesCache <- (Array.zeroCreate entities.Count)  
      entities.CopyTo(entitiesCache)
    entitiesCache



(** 
 * World
 *)
and World (totalComponents:int) =

  let isNull x = match x with null -> true | _ -> false

  let entities                          = new HashSet<Entity>(EntityEqualityComparer.comparer)
  let groups                            = new Dictionary<string,Group>()
  let groupsForIndex                    = (Array.zeroCreate (totalComponents+1))
  let reusableEntities                  = new Stack<Entity>()
  let retainedEntities                  = new HashSet<Entity>()
  let initializeSystems                 = new ResizeArray<IInitializeSystem>()
  let executeSystems                    = new ResizeArray<IExecuteSystem>()
  let mutable creationIndex             = 0
  let mutable entitiesCache             = (Array.zeroCreate 0)


  member this.totalComponents 
    with get() = totalComponents 
  member this.count 
    with get() = entities.Count
  member this.reusableEntitiesCount      
    with get() = reusableEntities.Count
  member this.retainedEntitiesCount      
    with get() = retainedEntities.Count

  [<DefaultValue>]
  static val mutable private _instance:World
  [<DefaultValue>]
  static val mutable private _nullEntity:Entity

  static member Create(totalComponents:int) =
    World._instance <- new World(totalComponents)
    World._nullEntity <- new Entity(World._instance)
    World._instance
  static member Instance with get() = World._instance
  static member NullEntity with get() = World._nullEntity

  //static Instance with get() = _instance

  (** 
   * CreateEntity
   *
   * @returns new entity
   *)
  member this.CreateEntity(name) =
    let mutable entity = 
      match reusableEntities.Count with
      | 0 -> new Entity(totalComponents+1, this)
      | _ -> reusableEntities.Pop()
    
    creationIndex <- creationIndex+1
    entity.Initialize(name, creationIndex)
    entity.Retain()
    entities.Add(entity) |> ignore    
    entitiesCache <- (Array.zeroCreate 0)
    entity

  (** 
   * DestroyEntity
   *
   * @param entity
   * @returns new entity
   *)
  member this.DestroyEntity(entity:Entity) =
    let removed = entities.Remove(entity)
    if not removed then 
      failwithf "Pool does not contain entity, could not destroy %s" (entity.ToString())

    entitiesCache <- (Array.zeroCreate 0)
    entity.destroy() |> ignore
    if entity.refCount = 1 then
      reusableEntities.Push(entity)
    else
      retainedEntities.Add(entity) |> ignore
    entity.Release()
  

  (** 
   * DestroyAllEntities
   *
   *)
  member this.DestroyAllEntities() =
    for entity in this.GetEntities() do
      this.DestroyEntity(entity)
    entities.Clear()
    if this.retainedEntitiesCount <> 0 then
      failwith "Pool still has retained entities" 
  
  (** 
   * HasEntity
   *
   * @param entity
   * @returns true if entity is found
   *)
  member this.HasEntity(entity) =
    entities.Contains(entity)

  (** 
   * GetEntities
   *
   * @returns array of entities
   *)
  member this.GetEntities() =
    if entitiesCache.Length = 0 then
      entitiesCache <- (Array.zeroCreate entities.Count)
      entities.CopyTo(entitiesCache)
    entitiesCache


  (** 
   * GetGroup
   *
   * @param matcher
   * @returns group for matcher
   *)
  member this.GetGroup(matcher) =
    match groups.ContainsKey(matcher.ToString()) with
    | true -> 
      groups.[matcher.ToString()]
    | _ ->
      let group = new Group(matcher:Matcher)
      for entity in this.GetEntities() do
        group.HandleEntitySilently(entity) |> ignore
      groups.Add(matcher.ToString(), group) |> ignore
      for index in matcher.Indices do
        if (isNull(groupsForIndex.[index])) then
          groupsForIndex.[index] <- new ResizeArray<Group>()
        groupsForIndex.[index].Add(group)
      group


  (** 
   * ResetCreationIndex
   *
   *)
  member this.ResetCreationIndex() =
    creationIndex <- 0

  interface IOwner with

    member this.onEntityReleased(entity:Entity) =
      if entity.IsEnabled then
        failwithf "Entity is not destroyed, cannot release entity %s" (entity.ToString())
      retainedEntities.Remove(entity) |> ignore
      reusableEntities.Push(entity)

    member this.onEntityChanged(entity:Entity, index:int, c:Component) =
      let groups = groupsForIndex.[index]
      if not(isNull(groups)) then
        for group in groups do
          group.HandleEntity(entity, index, c)


  (** 
   * Add 
   *
   * @param system
   *)
  member this.Add(system:obj) =
    match system with 
    | :? IInitializeSystem as initializeSystem ->
      initializeSystems.Add(initializeSystem)
    | _ -> ignore()

    match system with 
    | :? IExecuteSystem as executeSystem ->
      executeSystems.Add(executeSystem)
    | _ -> ignore()


  (** 
   * Initialize 
   *
   *)
  member this.Initialize() =
    for system in initializeSystems do
      system.Initialize()
  
  (** 
   * Execute
   *
   *)
  member this.Execute() =
    for system in executeSystems do
      system.Execute()

