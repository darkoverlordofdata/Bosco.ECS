namespace Entitas

  open System
  open System.Text
  open System.Collections.Generic

  [<AbstractClass>][<AllowNullLiteral>]
  type ASystem() = class end

  [<AbstractClass>]
  type IInitializeSystem() =
    inherit ASystem()
    abstract member Initialize: unit -> unit

  [<AbstractClass>]
  type IExecuteSystem() =
    inherit ASystem()
    abstract member Execute: unit -> unit


  [<AbstractClass>][<AllowNullLiteral>]
  type Component() = class end

  [<AbstractClass>][<AllowNullLiteral>]
  type AEntity() =

    abstract member Id:int with get,set
    abstract member IsEnabled:bool with get,set
    abstract member AddComponent: int * Component -> unit
    abstract member RemoveComponent: int -> unit
    abstract member ReplaceComponent: int * Component -> unit
    abstract member GetComponent: int -> Component
    abstract member GetComponents: unit -> Component[]
    abstract member HasComponent: int -> bool
    abstract member HasComponents: int[] -> bool
    abstract member HasAnyComponent: int[] -> bool
    abstract member RemoveAllComponents: unit -> unit
    abstract member Retain: Object -> unit
    abstract member Release: Object -> unit


  (** 
   * Matcher
   *)
  type Matcher () =

    let mutable _indices                  = Array.empty
    let mutable allOfIndices              = Array.empty
    let mutable anyOfIndices              = Array.empty
    let mutable noneOfIndices             = Array.empty
    let mutable toStringCache             = ""

    member val indices                    = _indices with get

    (** 
     * AnyOf 
     *
     * @param indices
     * @returns this 
     *)
    member this.AnyOf([<ParamArray>] indices: int[]) =
      anyOfIndices <- Matcher.distinctIndices(_indices)
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
      noneOfIndices <- Matcher.distinctIndices(_indices)
      _indices <- Array.empty
      this

    member this.NoneOf([<ParamArray>] matchers: Matcher[]) =
      this.NoneOf(Matcher.mergeIndices(matchers))
     
    (** 
     * Matches 
     *
     * @param entity
     * @returns true if entity is a match 
     *)
    member this.Matches(entity:AEntity) =
      let matchesAllOf = 
        match allOfIndices with 
        | null -> true
        | _ -> entity.HasComponents(allOfIndices)
      
      let matchesAnyOf =
        match anyOfIndices with 
        | null -> true
        | _ -> entity.HasComponents(anyOfIndices)

      let matchesNoneOf =
        match noneOfIndices with 
        | null -> true
        | _ -> not(entity.HasComponents(noneOfIndices))

      matchesAllOf && matchesAnyOf && matchesNoneOf
      

    (** 
     * mergeIndicse 
     *
     * @param matchers
     * @returns array of indices for the matchers
     *)
    static member mergeIndices(matchers:Matcher[]):int[] =
      let mutable indices = (Array.zeroCreate matchers.Length)
      for i=0 to matchers.Length do
        let matcher = matchers.[i]
        if matcher.indices.Length <> 1 then
          failwithf "Matcher indices length not = 1 %s" (matchers.[i].ToString())
        indices.[i] <- matcher.indices.[0]
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
        if allOfIndices.Length = 0 then
          sb.Append("AllOf") |> ignore
          sb.Append(allOfIndices.ToString()) |> ignore

        if anyOfIndices.Length = 0 then
          sb.Append("AnyOf") |> ignore
          sb.Append(anyOfIndices.ToString()) |> ignore

        if noneOfIndices.Length = 0 then
          sb.Append("NoneOf") |> ignore
          sb.Append(noneOfIndices.ToString()) |> ignore

      toStringCache

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
   
    [<DefaultValue>]val mutable singleEntityCache:AEntity
    let isNull x = match x with null -> true | _ -> false

    let onEntityAdded                     = new Event<GroupChangedDelegate, GroupChangedArgs>()
    let onEntityRemoved                   = new Event<GroupChangedDelegate, GroupChangedArgs>()
    let onEntityUpdated                   = new Event<GroupUpdatedDelegate, GroupUpdatedArgs>()
    let entities:HashSet<AEntity>         = new HashSet<AEntity>()
    let mutable entitiesCache             = Array.empty<AEntity>
    let mutable toStringCache             = ""

    member val OnEntityAdded              = onEntityAdded.Publish with get
    member val OnEntityRemoved            = onEntityRemoved.Publish with get
    member val OnEntityUpdated            = onEntityUpdated.Publish with get
    member val matcher                    = matcher with get
    member val count                      = entities.Count with get

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
        entitiesCache <- Array.empty<AEntity>
        this.singleEntityCache <- null //Array.empty<AEntity>
        entity.Retain(this)
      added

    (** 
     * removeEntitySilently
     *
     * @param entity
     *)
    member this.removeEntitySilently(entity) =
      let removed = entities.Remove(entity)
      if removed then
        entitiesCache <- Array.empty<AEntity>
        this.singleEntityCache <- null //Array.empty<AEntity>
        entity.Retain(this)
      removed

    //member this.addEntity(entity) =
    //  match addEntitySilently(entity) with
    //  | true -> this.OnEntityAdded
    //  | _ -> null

    member this.addEntity(entity:AEntity, index, comp) =
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
        onEntityAdded.Trigger(this, new GroupChangedArgs(entity, index, comp))

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
      if isNull(this.singleEntityCache) then
        match entities.Count with
        | 1 ->
          let mutable enumerator = entities.GetEnumerator()
          enumerator.MoveNext() |> ignore
          this.singleEntityCache <- enumerator.Current
        | 0 ->
          this.singleEntityCache <- null
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
        entitiesCache  <- (Array.zeroCreate entities.Count)  
        entities.CopyTo(entitiesCache)
      entitiesCache



  (** 
   * Systems
   *)
  type Systems () =

    let _initializeSystems = new ResizeArray<IInitializeSystem>()
    let _executeSystems = new ResizeArray<IExecuteSystem>()

    (** 
     * Add 
     *
     * @param system
     *)
    member this.Add(system:ASystem) =
      match system with 
      | :? IInitializeSystem as initializeSystem ->
        _initializeSystems.Add(initializeSystem)
      | _ -> ignore()

      match system with 
      | :? IExecuteSystem as executeSystem ->
        _executeSystems.Add(executeSystem)
      | _ -> ignore()


    (** 
     * Initialize 
     *
     *)
    member this.Initialize() =
      for system in _initializeSystems do
        system.Initialize()
    
    (** 
     * Execute
     *
     *)
    member this.Execute() =
      for system in _executeSystems do
        system.Execute()


  (** 
   * Entity Events
   *)
  type EntityReleasedArgs(entity:AEntity) =
    inherit System.EventArgs() 
    member this.entity = entity

  type ComponentAddedArgs(entity:AEntity, index, newComponent) =
    inherit System.EventArgs()
    member this.entity = entity
    member this.index = index
    member this.newComponent = newComponent

  type ComponentRemovedArgs(entity:AEntity, index, previous) =
    inherit System.EventArgs()
    member this.entity = entity
    member this.index = index
    member this.previous = previous

  type ComponentReplacedArgs(entity:AEntity, index, previous, replacement) =
    inherit System.EventArgs()
    member this.entity = entity
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
  type Entity (totalComponents:int) =
    inherit AEntity()

    let isNull x = match x with null -> true | _ -> false

    let onComponentAdded                  = new Event<ComponentAddedDelegate, ComponentAddedArgs>()
    let onComponentRemoved                = new Event<ComponentRemovedDelegate, ComponentRemovedArgs>()
    let onComponentReplaced               = new Event<ComponentReplacedDelegate, ComponentReplacedArgs>()
    let onEntityReleased                  = new Event<EntityReleasedDelegate, EntityReleasedArgs>()
    let components: Component array       = (Array.zeroCreate totalComponents)
    let owners                            = new HashSet<Object>() 
    let mutable componentsCache           = Array.empty<Component> //new ResizeArray<Component>()
    let mutable toStringCache             = "" 

    member val retainCount                = owners.Count with get
    member val OnComponentAdded           = onComponentAdded.Publish with get
    member val OnComponentRemoved         = onComponentRemoved.Publish with get
    member val OnComponentReplaced        = onComponentReplaced.Publish with get
    member val OnEntityReleased           = onEntityReleased.Publish with get

    override val Id                       = 0 with get, set
    override val IsEnabled                = false with get, set
         
     (** 
     * AddComponent 
     *
     * @param index
     * @param component
     * @returns this entity
     *)
    override this.AddComponent(index:int, c:Component) =
      if not this.IsEnabled then failwith "Entity is disabled, cannot add component"
      if this.HasComponent(index) then failwithf "Entity already has component, cannot add at index %d" index

      components.[index] <- c
      componentsCache <- Array.empty<Component>
      toStringCache <- ""
      onComponentAdded.Trigger(this, new ComponentAddedArgs(this, index, c))
    
    (** 
     * RemoveComponent 
     *
     * @param index
     * @returns this entity
     *)
    override this.RemoveComponent(index:int) =
      if not this.IsEnabled then failwith "Entity is disabled, cannot remove component"
      if not(this.HasComponent(index)) then failwithf "Entity does not have component, cannot remove at index %d" index

      this.replaceComponent(index, null)

    (** 
     * ReplaceComponent 
     *
     * @param index
     * @param component
     * @returns this entity
     *)
    override this.ReplaceComponent(index:int, c:Component) =
      if not this.IsEnabled then failwith "Entity is disabled, cannot replace at index %d" index
      if this.HasComponent(index) then
        this.replaceComponent(index, c)
      else
        this.AddComponent(index, c) |> ignore

    (** 
     * GetComponent 
     *
     * @param index
     * @returns the component at index
     *)
    override this.GetComponent(index:int) =
      if not(this.HasComponent(index)) then failwithf "Entity does not have component, cannot get at index %d" index
      components.[index]

    (** 
     * GetComponents 
     *
     * @returns a list of components
     *)
    override this.GetComponents() =
      if componentsCache.Length = 0 then
        componentsCache <- Array.filter ((<>)null) components
      componentsCache

    (** 
     * HasComponent
     *
     * @param index
     * @returns true if entity has component at index
     *)
    override this.HasComponent(index:int) =
      not(isNull(components.[index]))


    (** 
     * HasComponents
     *
     * @param indices array
     * @returns true if entity has all components in array
     *)
    override this.HasComponents(indices:int[]) =
      let mutable i = 0
      let mutable flag = true

      while (flag && i<indices.Length) do
        if isNull(components.[indices.[i]]) then
          flag <- false
        i <- i+1
      flag

    (** 
     * HasAnyComponent
     *
     * @param indices array
     * @returns true if entity has any component in array
     *)
    override this.HasAnyComponent(indices:int[]) =
      let mutable i = 0
      let mutable flag = false

      while (flag && i<indices.Length) do
        if not(isNull(components.[indices.[i]])) then
          flag <- true
        i <- i+1
      flag

    (** 
     * RemoveAllComponents
     *
     *)
    override this.RemoveAllComponents() =
      
      for i = 0 to components.Length do
        if not(isNull(components.[i])) then
          components.[i] <- null
      ()


    (** 
     * Retain (reference count)
     *
     *)
    override this.Retain(owner:Object) =
      if not(owners.Add(owner)) then
        failwithf "Entity is alread retained by %s" (owner.ToString())

    (** 
     * Release (reference count)
     *
     *)
    override this.Release(owner:Object) =
      if not(  owners.Remove(owner)) then
        failwithf "Entity was not retained by %s" (owner.ToString())
   
      if owners.Count = 0 then
        onEntityReleased.Trigger(this, new EntityReleasedArgs(this))

    (** 
     * ToString
     *
     *)
    override this.ToString() =
      if toStringCache = "" then

        let sb = new StringBuilder()
        sb.Append("Entity_") |> ignore
        sb.Append(this.Id.ToString()) |> ignore
        sb.Append("(") |> ignore
        sb.Append(this.retainCount.ToString()) |> ignore
        sb.Append(")") |> ignore
        sb.Append("(") |> ignore
        for i = 0 to components.Length-1 do
          if not(isNull(components.[i])) then
              sb.Append(components.[i].GetType().ToString()) |> ignore
              if i < components.Length-1 then sb.Append(",") |> ignore
        sb.Append(")") |> ignore
        toStringCache <- sb.ToString()

      toStringCache

    (** 
     * destroy an entity
     *
     *)
    member this.destroy() =
      (this:>AEntity).RemoveAllComponents()
      (this:>AEntity).IsEnabled = false

    (** 
     * replaceComponent 
     *
     * @param index
     * @param component
     *)
    member private this.replaceComponent(index, replacement) =
      let previousComponent = components.[index]
      if obj.ReferenceEquals(previousComponent, replacement) then
        onComponentReplaced.Trigger(this, new ComponentReplacedArgs(this, index, previousComponent, replacement))
      else
        components.[index] <- replacement
        componentsCache <- Array.empty<Component> 
        toStringCache <- ""
        if obj.ReferenceEquals(replacement, null) then
          onComponentRemoved.Trigger(this, new ComponentRemovedArgs(this, index, previousComponent))
        else
          onComponentReplaced.Trigger(this, new ComponentReplacedArgs(this, index, previousComponent, replacement))


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
    let entities                          = new HashSet<Entity>()
    let groups                            = new Dictionary<string,Group>()
    let groupsForIndex                    = (Array.zeroCreate totalComponents)
    let reusableEntities                  = new Stack<AEntity>()
    let retainedEntities                  = new HashSet<AEntity>()
    let mutable creationIndex             = 0
    let mutable entitiesCache             = (Array.zeroCreate 0)

    member val OnEntityCreated            = onEntityCreated.Publish with get
    member val OnEntityWillBeDestroyed    = onEntityWillBeDestroyed.Publish with get
    member val OnEntityDestroyed          = onEntityDestroyed.Publish with get
    member val OnGroupCreated             = onGroupCreated.Publish with get
    member val OnGroupCleared             = onGroupCleared.Publish with get
    member val totalComponents            = totalComponents with get
    member val count                      = entities.Count with get
    member val reusableEntitiesCount      = reusableEntities.Count with get
    member val retainedEntitiesCount      = retainedEntities.Count with get

    (** 
     * CreateEntity
     *
     * @returns new entity
     *)
    member this.CreateEntity() =
      let mutable entity = 
        match reusableEntities.Count with
        | 0 -> new Entity(totalComponents)
        | _ -> reusableEntities.Pop():?>Entity
      
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
      if entity.retainCount = 1 then
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
        for index in matcher.indices do
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
        let entity = evt.entity:?>Entity
        printf "hello"
      )

    (** 
     * OnComponentRemoved
     *
     *)
    member this.updateGroupsComponentRemoved =
      new ComponentRemovedDelegate(fun sender evt ->
        let entity = evt.entity:?>Entity
        printf "hello"
      )

    (** 
     * OnComponentReplaced
     *
     *)
    member this.updateGroupsComponentReplaced =
      new ComponentReplacedDelegate(fun sender evt ->
        let groups = groupsForIndex.[evt.index]
        if not(isNull(groups)) then
          for group in groups do
            group.UpdateEntity(evt.entity, evt.index, evt.previous, evt.replacement)
      )

    (** 
     * OnComponentReleased
     *
     *)
    member this.onEntityReleased =
      new EntityReleasedDelegate (fun sender evt ->
        let entity = evt.entity:?>Entity

        if entity.IsEnabled then
          failwith "Entity is not destroyed, cannot release entity"
        
        entity.OnEntityReleased.RemoveHandler(this.onEntityReleased)
        retainedEntities.Remove(entity) |> ignore
        reusableEntities.Push(entity)
      )

