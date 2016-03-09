
LIGHTMAPPING EXTENDED

created by Michael Stevenson
michael@mstevenson.net


Lightmapping Extended is an editor tool that exposes all compatible XML configuration options for Unity's integrated Autodesk Beast lightmapper through a simple to use UI. The most notable additions to Unity's built-in settings are image-based lighting, Path Tracer GI, and Monte Carlo GI.

Full source is available on GitHub: https://github.com/mstevenson/Lightmapping-Extended


Setup
-----

The Lightmapping Extended editor window is accessible from the menu "Window > Lightmapping Extended".

Lightmapping configuration settings are stored as a separate XML file for each scene. If the current scene does not include a configuration file, the Lightmapping Extended editor window will provide an option to create one. If a configuration file already exists for the current scene it will be automatically loaded, otherwise Unity's built-in lightmapping settings will be used.


Presets
-------

Configuration settings may be saved as presets. Presets are stored in the folder "Lightmapping Extended/Presets" and may be checked into source control. Presets are available per-project, making them easy to re-use across multiple scenes. Preset files may be organized into folders and will be displayed hierarchically in the Presets selection menu at the top of the Lightmapping Extended window.


Shaders
-------

A set of transmissive shaders are included with Lightmapping Extended. These shaders are identical to Unity's built-in transparent shaders, with the addition of a "Transmissive Color" material property. A texture may be used to define which colors of light are able to pass through the material, producing colored shadows. Colored shadows are only supported in baked lightmaps and will not be displayed by Unity's real-time shadow system.