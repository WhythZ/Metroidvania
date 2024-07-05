# Metroidvania

## LICENSE
This repo adopts [MIT License](https://spdx.org/licenses/MIT)

## About
This repo is a ***Metroidvania*** game prototype developed by Unity2D

We have constructed multiple extensible systems (see the [combat showcase](https://www.bilibili.com/video/BV1eb421E7bN/?spm_id_from=333.999.list.card_archive.click&vd_source=6fbd8ea5d181239758f62d5f9a9d8dfb))

- Entity (Player&Enemy) system
    - Finite state machine
        - Entity behavior
        - Entity animation
    - Entity manager
        - Player manager
        - Enemy manager
    - Skill system
        - Player skill
            - Dash
            - Wall slide
            - Throw sword
            - Elemental ball
            - Blackhole
            - Clone
        - Enemy skill
    - Statistics system
        - Health
        - Attributes
        - Attack
        - Defence
    - Buff system
        - Ignited
        - Chilled
        - Shocked
    - Visual effect system
        - Buff FX
        - Hit FX
        - Critical hit FX

- UI system
    - Ingame UI
        - Dynamic health bar
        - Dynamic skill cooldown panal
    - Inventory UI
        - Item slots
        - Stat slots
        - Description tooltip
    - Settings UI
        - Audio volume settings
        - Game save button
    - Skills UI
    - Interact UI

- Inventory system
    - Item system
    - Storage system

- Interact system
    - Checkpoint
    - Jukebox
    - Portal
    - Enemy spawner
    - Dead zone

- Audio system
    - Background music
    - Sound effect

- Save&Load system
    - Game data file
    - File data handler

## Deployment
Follow the steps to deploy this project locally
- Install the corresponding Unity version as `2022.3.17f1c1`
- Create a new Unity 2D project, exit after successfully entering it
- Replace by these files in the repo

The game save will be stored in the directory
```
C:\Users\your_user_name\AppData\LocalLow\DefaultCompany\your_project_name\data.whythz
```

You can build the game using `Ctrl + B` to get the executable file 

## References
- My loved games and anime
- [Unity Manual](https://docs.unity.cn/2021.3/Documentation/Manual/UnityManual.html)
- [itch.io](https://itch.io/)
- [udemy.com](https://www.udemy.com/course/2d-rpg-alexdev/)
- [bilibili.com](https://www.bilibili.com/)