* Bosco.ECS


A lightweight ECS inspired by Entitas

Fixed resuable entity pooling bug*
Entity is no longer nullable


* 'retainedEntities' is not being used in entitas-ts, and using it here causes a bug.
For now it is commented out. Bosco.ECS does not implement reactive systems, so retainedEntities 
may not be a required feature