# My Beloved Collection

**My Beloved Collection** is a 2D action side-scroller built in **Unity (C#)**, focusing on combat feel, enemy systems, and metroidvania-style progression design.

> **Status:** On hold (project dropped, planned for future revisit)

## Story
Upon his death when looking for "content" on his PC late one night, Jeoff pleaded with the devil to send him back to Earth so that he may settle some unfinished business. The devil accepted his (rather pathetic) plead and offer him a challange.
For if he is able to finish a set of challanges, he will be given temporary pass back to earth. This begin Jeoff's journey to destory his Beloved Collection.

## Overview

This project explores tight combat design, ability-gated progression, and systemic gameplay structure inspired by classic side-scrolling action games.  
The main focus is on responsive player feedback, enemy behavior design, and structured level progression.

## Features

- **Player combat and movement systems** designed with a strong focus on responsiveness and game feel
- **Multi-channel feedback system** using audio, particles, camera shake, and UI to enhance combat satisfaction
- **Enemy system using polymorphism**, allowing shared base behavior with varied derived enemy reactions
- **Boss fight system using state machines**, supporting phase transitions and structured attack patterns
- **Metroidvania-style level progression**, where abilities gate access to new sections of the world
- **Custom room-based rendering/culling system** to optimize performance in segmented level layouts

## Core Systems

### Player Feedback & Combat Feel
Player movement and combat are reinforced through layered feedback systems including:
- Camera shake
- Particle effects (movement and attacks)
- Audio cues
- Visual and UI feedback

This system was designed to make basic actions like movement and attacking feel impactful and responsive.

### Enemy Architecture
Enemies are built using a **polymorphic base class structure**, allowing shared core behavior while enabling unique reactions and variations through derived implementations.

### Boss System
Boss encounters use a **state machine architecture**, supporting:
- Attack pattern switching
- Phase transitions based on health thresholds
- Controlled variation in behavior to avoid repetition

### Level Progression
Levels are structured around **ability-gated progression**, where players unlock access to new areas through acquired abilities.  
This creates a structured exploration loop typical of metroidvania-inspired design.

### Performance Optimization
A custom **room-based culling system** was implemented to improve performance in segmented levels by only rendering nearby rooms based on player position.

## Tech Stack

- Unity
- C#
- State Machines
- Polymorphism
- Object-Oriented Design
- Custom Rendering / Culling Logic

## Notes

This project is currently on hold, but is intended to be revisited in the future for further development and refinement.
