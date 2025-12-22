# 3D-Portal Recreation
Portal recreation made in Unity for learning more about its mechanics.

## Video
[![Image](https://img.youtube.com/vi/drSybE8P77w/0.jpg)](https://www.youtube.com/watch?v=drSybE8P77w)

## Features
- Portal Gun
    - Portal preview before shooting ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/PortalPlacing.gif)

    - Portals can only be placed in valid surfaces ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/ValidPlacing.gif)

    - The player can resize the portal to 50%, 100% and 200% its original size ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/PortalResize.gif)

    - Crosshair changes based on which portals are placed ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/Crosshair.gif)

- Each portal reflects the other portal's "POV" ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/PortalRender.gif)

- Player teleports by going through the portals ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/PlayerTp.gif)


- Companion Cubes
    - Come from a spawner and disappear when another one is spawned from the same source ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/CubeSpawner.gif)
      
    - Can activate buttons to open doors if placed on them ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/CubeButton.gif)

      
    - Can teleport and will get bigger or smaller based on the size of each portal ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/CubeResize.gif)
      
- Turrets
    - Turrets shoot a laser beam ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/TurretLaser.gif)
      
    - Lasers can kill the player or other turrets ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/TurretKill.gif)

    - Turrets can teleport, but will not change their size by doing so
      
    - Lasers can also go through portals ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/LaserTP.gif)

      
- Refraction Cubes
    - Reflect laser beams if hit by them ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/RefractionCube.gif)

    - Can teleport the same way turrets can
      
- Physic Surfaces
    - Bouncing surface ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/BouncingMaterial.gif)

    - Sliding surface ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/SlidingMaterial.gif)
      
- Deadzones for the player and / or objects ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/PlayerDeadzone.gif) ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/ItemDeadzone.gif)

- Simple checkpoint system ![Image](https://github.com/shikkenzo/3D-PortalRecreation/blob/main/Resources/Checkpoint.gif)
  
- Game Reset system using Interfaces

## Tools
- Unity 6000.2.6f2
    - Package: Visual Studio Editor
    - Package: Unity UI

## Controls
- Move: W, A, S, D
- Run: Left Shift
- Shoot Blue Portal / Throw Object: Left Click
- Shoot Orange Portal / Drop Object: Right Click
- Grab / Interact: E

