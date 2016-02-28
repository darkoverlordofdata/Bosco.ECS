* Bosco.ECS


A lightweight ECS inspired by Entitas.
Bosco is bare bones, and doesn't have reactive systems or monitoring.
Code is generated from entitas.js using the entitas-ts cli at https://github.com/darkoverlordofdata/entitas-ts. Entitas-ts requres NodeJS.

Demo Shmupwarz provides the game logic dlls for https://github.com/darkoverlordofdata/shmupwarz-unity

Fixed resuable entity pooling bug*
Entity is no longer nullable


* 'retainedEntities' is not being used in entitas-ts, and using it here causes a bug.
For now it is commented out. Bosco.ECS does not implement reactive systems, so retainedEntities 
may not be a required feature