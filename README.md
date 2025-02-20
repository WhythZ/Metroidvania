# Metroidvania

## License
This repo adopts [MIT License](https://spdx.org/licenses/MIT)

## About
In this game prototype, you can control the character in agile locomotion, unlock 5+ abilities and attack skills to support your exploration and battling with enemies, [click to play online](https://whythz-debug.github.io/Metroidvania/) or [see the showcase](https://www.bilibili.com/video/BV1fM4m1171d/?share_source=copy_web&vd_source=5ef86699cafaaf10c5dc362759c73a7d)

There are multiple extensible gameplay systems constructed in this prototype, notice that `Entity` class derives `Player` and `Enemy`, the latter derives concrete enemies
- Entity System
    - Finite State Machine
        - Entity Behavior
        - Entity Animation
    - Skill System
    - Statistics System
    - Buff System
    - Visual Effect System
- UI System
    - Ingame UI
    - Character UI
    - Skill Tree UI
    - Settings UI
    - Interactive Object UI
- Inventory System
    - Item System
    - Storage System
- Audio System
    - Background Music
    - Sound Effect
- Save&Load System
    - Game Configs File
    - File Data Handler

## Deployment
|Software|Version|
|---|---|
|Unity Editor|2022.3.17f1c1|
|Visual Studio|2022|
|Windows|10/11|

Use `Ctrl + B` to build the game and get the executable file 

The game save will be stored in the directory
```
C:\Users\your_user_name\AppData\LocalLow\DefaultCompany\your_project_name\data.whythz
```

## Acknowledgement
- [Tutorial](https://www.udemy.com/course/2d-rpg-alexdev/): teaching me basic unity skills
- ttyclear: supporting on skill implementation
- Elaina: supporting on enemy implementation
- NSPoison: supporting on building game scene
- GirlsBandCry: keeping me company with melodies since beginning of the project
