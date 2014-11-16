Karts of Chaos - Unity game
============================

Unity game repository for the game [Karts of Chaos](https://github.com/kartsofchaos/game).

Cloning the repository
----------------------

This repository can both be cloned with [Git](http://git-scm.com/) or [Subversion](https://subversion.apache.org/).

Using Git:
- `git clone https://github.com/kartsofchaos/game`
- [GUI clients](http://git-scm.com/downloads/guis)

Using Subversion:
- `svn checkout https://github.com/kartsofchaos/game`
- [TortoiseSVN](http://tortoisesvn.net/)

Directory structure
-------------------

To keep structure of the Unity project the following (preliminary) structure should be followed:

Contents of `Assets` folder:
- `Editor`
- `Extensions`
- `Fonts`
- `Game`
  - `Actors`
    - `Code`
    - `Resources`
      - `Models`
      - `Prefabs`
      - `Sounds`
      - `Textures`
  - `Environment`
    - `Code`
    - `Resources`
      - `Models`
      - `Prefabs`
      - `Sounds`
      - `Textures`
  - `GameModes`
    - `Code`
    - `Resources`
      - `Models`
      - `Prefabs`
      - `Sounds`
      - `Textures`
  - `HUD`
    - `Code`
    - `Resources`
      - `Prefabs`
      - `Textures`
  - `Items`
    - `Code`
    - `Resources`
      - `Models`
      - `Prefabs`
      - `Sounds`
      - `Textures`
  - `Menus`
    - `Code`
    - `Resources`
      - `Skins`
      - `Sounds`
      - `Textures`
  - `Networking`
    - `Code`
    - `Resources`
  - `Skills`
    - `Code`
    - `Resources`
      - `Models`
      - `Prefabs`
      - `Sounds`
      - `Textures`
- `Plugins`
- `Scenes`
- `Standard Assets`

File formats
------------

The following file format should be followed:

- Script files - `.cs` (C#)

Naming convention
-----------------

The naming convention to follow are [CamelCase](http://en.wikipedia.org/wiki/CamelCase). This means that each next word or phrase should begin with a capital letter, and the spaces are left out, for example: `ThisIsAScript.cs`. 
