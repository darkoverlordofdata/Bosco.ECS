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

type GroupUpdatedArgs(entity, index, prevComponent, newComponent) =
  inherit System.EventArgs()
  member this.entity = entity
  member this.index = index
  member this.prevComponent = prevComponent
  member this.newComponent = newComponent

type GroupChangedDelegate = delegate of obj * GroupChangedArgs -> unit
type GroupUpdatedDelegate = delegate of obj * GroupUpdatedArgs -> unit

(** 
 * Group
 *)
type Group (matcher:Matcher) =
 
  let mutable singleEntityCache:Entity = null
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
      singleEntityCache <- null
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
      singleEntityCache <- null
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
    if isNull(singleEntityCache) then
      match entities.Count with
      | 1 ->
        use mutable enumerator = entities.GetEnumerator()
        enumerator.MoveNext() |> ignore
        singleEntityCache <- enumerator.Current
      | 0 ->
        singleEntityCache <- null
      | _ ->
        failwithf "Single Entity Execption %s" (matcher.ToString())
    singleEntityCache
  
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



