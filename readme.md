* Bosco

Bosco - Some extra utilities for Unity

* JSON - port of javascript's JSON routines
* PlayerPrefsDB - A lightweight DB implemented using JSON
* Properties - Game Properties DB
* Timer - A timer object
* TrigLUT - Trig lookup tables for 2D

Bosco.ECS - Entity Component System Fx for Unity


Bosco.ECS is a lightweight entity component system framework inspired by Entitas.
Bosco.ECS is bare bones, and doesn't have reactive systems or monitoring.
Code is generated from entitas.js using the entitas-ts cli at https://github.com/darkoverlordofdata/entitas-ts. Entitas-ts requres NodeJS.

Demo Shmupwarz provides the game logic dlls for https://github.com/darkoverlordofdata/shmupwarz-unity

Fixed resuable entity pooling bug*
Entity is no longer nullable


* 'retainedEntities' is not being used in entitas-ts, and using it here causes a bug.
For now it is commented out. Bosco.ECS does not implement reactive systems, so retainedEntities 
may not be a required feature