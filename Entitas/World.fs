namespace Entitas

open System
open System.Text
open System.Collections.Generic


(** 
 * World Events
 *)
type EntityEventArgs(entity) =
  inherit System.EventArgs()
  member this.entity = entity

type GroupEventArgs(group) =
  inherit System.EventArgs()
  member this.group = group

type GroupCreatedDelegate = delegate of obj * GroupEventArgs -> unit
type GroupClearedDelegate = delegate of obj * GroupEventArgs -> unit
type EntityCreatedDelegate = delegate of obj * EntityEventArgs -> unit
type EntityWillBeDestroyedDelegate = delegate of obj * EntityEventArgs -> unit
type EntityDestroyedDelegate = delegate of obj * EntityEventArgs -> unit



(** 
 * World
 *)
type World (totalComponents:int) =
      
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

  (** 
   * CreateEntity
   *
   * @returns new entity
   *)
  member this.CreateEntity() =
    let mutable entity = 
      match reusableEntities.Count with
      | 0 -> new Entity(totalComponents+1)
      | _ -> reusableEntities.Pop()
    
    entity.IsEnabled <- true
    entity.Id <- creationIndex+1
    entity.OnComponentAdded.AddHandler(this.updateGroupsComponentAdded)
    entity.OnComponentRemoved.AddHandler(this.updateGroupsComponentRemoved)
    entity.OnComponentReplaced.AddHandler(this.updateGroupsComponentReplaced)
    entity.OnEntityReleased.AddHandler(this.onEntityReleased)
    creationIndex <- entity.Id
    entities.Add(entity) |> ignore
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
    if not removed then failwith "Pool does not contain entity, could not destroy"
    onEntityWillBeDestroyed.Trigger(this, new EntityEventArgs(entity))
    entity.destroy() |> ignore
    onEntityDestroyed.Trigger(this, new EntityEventArgs(entity))
    if entity.refCount = 1 then
      entity.OnEntityReleased.RemoveHandler(this.onEntityReleased)
      reusableEntities.Push(entity)
    else
      retainedEntities.Add(entity) |> ignore
    entity.Release(this)
  

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
        group.GetEntities().[i].Release(group)
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

      //if entity.IsEnabled then
      //  failwith "Entity is not destroyed, cannot release entity"
      
      entity.OnEntityReleased.RemoveHandler(this.onEntityReleased)
      retainedEntities.Remove(entity) |> ignore
      reusableEntities.Push(entity)
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

