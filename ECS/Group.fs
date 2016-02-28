namespace Bosco.ECS

open System
open System.Text
open System.Collections.Generic

(** 
 * Group Events
 *)
type GroupChangedArgs(entity, index, newComponent) =
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
and GroupUpdatedDelegate = delegate of obj * GroupUpdatedArgs -> unit

(** 
 * Group
 *)
and Group (matcher:Matcher) =
 
  [<DefaultValue>] val mutable singleEntityCache:Entity
  [<DefaultValue>] val mutable singleEntityCacheFlag:bool
  let isNull x = match x with null -> true | _ -> false

  let onEntityAdded                     = new Event<GroupChangedDelegate, GroupChangedArgs>()
  let onEntityRemoved                   = new Event<GroupChangedDelegate, GroupChangedArgs>()
  let onEntityUpdated                   = new Event<GroupUpdatedDelegate, GroupUpdatedArgs>()
  let entities:HashSet<Entity>          = new HashSet<Entity>(EntityEqualityComparer.comparer)
  let mutable entitiesCache             = Array.empty<Entity>
  let mutable toStringCache             = ""

  member val OnEntityAdded              = onEntityAdded.Publish with get
  member val OnEntityRemoved            = onEntityRemoved.Publish with get
  member val OnEntityUpdated            = onEntityUpdated.Publish with get
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

  //member internal this.handleEntity(entity) =
  //  match matcher.Matches(entity) with
  //  | true -> this.addEntity(entity)
  //  | _ -> this.removeEntity(entity)


  (** 
   * HandleEntity
   *
   * @param entity
   * @param index
   * @param previous component
   * @paran new component
   *)
  member this.UpdateEntity(entity, index, previousComponent, newComponent) =
    if entities.Contains(entity) then
      onEntityAdded.Trigger(this, new GroupChangedArgs(entity, index, previousComponent))
      onEntityAdded.Trigger(this, new GroupChangedArgs(entity, index, newComponent))
      onEntityUpdated.Trigger(this, new GroupUpdatedArgs(entity, index, previousComponent, newComponent))

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
      | 0 -> // return a dummy 'null' entity
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
 * World Events
 *)
and EntityEventArgs(entity) =
  inherit System.EventArgs()
  member this.entity = entity

and GroupEventArgs(group) =
  inherit System.EventArgs()
  member this.group = group

and GroupCreatedDelegate = delegate of obj * GroupEventArgs -> unit
and GroupClearedDelegate = delegate of obj * GroupEventArgs -> unit
and EntityCreatedDelegate = delegate of obj * EntityEventArgs -> unit
and EntityWillBeDestroyedDelegate = delegate of obj * EntityEventArgs -> unit
and EntityDestroyedDelegate = delegate of obj * EntityEventArgs -> unit



(** 
 * World
 *)
and World (totalComponents:int) =

  let isNull x = match x with null -> true | _ -> false

  let onEntityCreated                   = new Event<EntityCreatedDelegate, EntityEventArgs>() 
  let onEntityWillBeDestroyed           = new Event<EntityWillBeDestroyedDelegate, EntityEventArgs>()
  let onEntityDestroyed                 = new Event<EntityDestroyedDelegate, EntityEventArgs>()
  let onGroupCreated                    = new Event<GroupCreatedDelegate, GroupEventArgs>()
  let onGroupCleared                    = new Event<GroupClearedDelegate, GroupEventArgs>()
  let entities                          = new HashSet<Entity>(EntityEqualityComparer.comparer)
  let groups                            = new Dictionary<string,Group>()
  let groupsForIndex                    = (Array.zeroCreate (totalComponents+1))
  let reusableEntities                  = new Stack<Entity>()
  let retainedEntities                  = new HashSet<Entity>()
  let initializeSystems                 = new ResizeArray<IInitializeSystem>()
  let executeSystems                    = new ResizeArray<IExecuteSystem>()
  let mutable creationIndex             = 0
  let mutable entitiesCache             = (Array.zeroCreate 0)

  member val OnEntityCreated            = onEntityCreated.Publish with get
  member val OnEntityWillBeDestroyed    = onEntityWillBeDestroyed.Publish with get
  member val OnEntityDestroyed          = onEntityDestroyed.Publish with get
  member val OnGroupCreated             = onGroupCreated.Publish with get
  member val OnGroupCleared             = onGroupCleared.Publish with get

  member this.totalComponents 
    with get() = totalComponents 
  member this.count 
    with get() = entities.Count
  member this.reusableEntitiesCount      
    with get() = reusableEntities.Count
  member this.retainedEntitiesCount      
    with get() = retainedEntities.Count

  member this.ReusableEntities
    with get() = reusableEntities

  [<DefaultValue>]
  static val mutable private _instance:World
  [<DefaultValue>]
  static val mutable private _nullEntity:Entity

  static member Create(totalComponents:int) =
    World._nullEntity <- new Entity()
    World._instance <- new World(totalComponents)
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
      | 0 -> 
        new Entity(totalComponents+1)
      | _ -> 
        reusableEntities.Pop()

    entity.IsEnabled <- true
    entity.Id <- creationIndex+1
    entity.Name <- name
    entity.Retain()
    entity.OnComponentAdded.AddHandler(this.updateGroupsComponentAdded)
    entity.OnComponentRemoved.AddHandler(this.updateGroupsComponentRemoved)
    entity.OnComponentReplaced.AddHandler(this.updateGroupsComponentReplaced)
    entity.OnEntityReleased.AddHandler(this.onEntityReleased)
    creationIndex <- entity.Id
    entities.Add(entity) |> ignore    
    entitiesCache <- (Array.zeroCreate 0)
    onEntityCreated.Trigger(this, new EntityEventArgs(entity))
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
    onEntityWillBeDestroyed.Trigger(this, new EntityEventArgs(entity))
    entity.destroy() |> ignore
    onEntityDestroyed.Trigger(this, new EntityEventArgs(entity))
    if entity.refCount = 1 then
      entity.OnEntityReleased.RemoveHandler(this.onEntityReleased)
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
      onGroupCreated.Trigger(this, new GroupEventArgs(group))
      group

  (** 
   * ClearGroup
   *
   *)
  member this.ClearGroups() =
    for group in groups.Values do
      for i=0 to group.GetEntities().Length-1 do
        group.GetEntities().[i].Release()
      onGroupCleared.Trigger(this, GroupEventArgs(group))

    groups.Clear()
    for i=0 to groupsForIndex.Length-1 do
      groupsForIndex.[i] <- null


  (** 
   * ResetCreationIndex
   *
   *)
  member this.ResetCreationIndex() =
    creationIndex <- 0

  (** 
   * OnComponentAdded
   *
   *)
  member this.updateGroupsComponentAdded =
    new ComponentAddedDelegate(fun sender evt ->
      let groups = groupsForIndex.[evt.index]
      if not(isNull(groups)) then
        for group in groups do
          group.HandleEntity(sender:?>Entity, evt.index, evt.newComponent)
    )

  (** 
   * OnComponentRemoved
   *
   *)
  member this.updateGroupsComponentRemoved =
    new ComponentRemovedDelegate(fun sender evt ->
      let groups = groupsForIndex.[evt.index]
      if not(isNull(groups)) then
        for group in groups do
          group.HandleEntity(sender:?>Entity, evt.index, evt.previous)
    )

  (** 
   * OnComponentReplaced
   *
   *)
  member this.updateGroupsComponentReplaced =
    new ComponentReplacedDelegate(fun sender evt ->
      let groups = groupsForIndex.[int evt.index]
      if not(isNull(groups)) then
        for group in groups do
          group.UpdateEntity(sender:?>Entity, evt.index, evt.previous, evt.replacement)
    )

  (** 
   * OnComponentReleased
   *
   *)
  member this.onEntityReleased =
    new EntityReleasedDelegate (fun sender evt ->
      let entity = sender:?>Entity

      if entity.IsEnabled then
        failwithf "Entity is not destroyed, cannot release entity %d/%s" (entity.refCount) (entity.ToString())

      entity.OnEntityReleased.RemoveHandler(this.onEntityReleased)
      retainedEntities.Remove(entity) |> ignore
      //reusableEntities.Push(entity)
    )

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


