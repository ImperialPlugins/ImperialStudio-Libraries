# ImperialStudio Libraries
Contains a modern framework for game projects.
All libraries (except ImperialStudio.Core.UnityEngine) are written in an engine-independent way. 

Some features include:
* ENet based high level networking API with an authorative server (no P2P support at the moment)
* IoC container for services
* Abstractions. A lot of them. Any component is replacable.
* A Unity project/library that actually uses modern C# with lots of OOP, patterns, etc.

# Compiling
If you want to compile the Unity projects, copy "UnityEngine.CoreModule.dll", "UnityEngine.PhysicsModule.dll", "UnityEngine.ProfilerModule.dll" and "UnityEngine.dll" into the Libraries folder. If you are using an older version of Unity, just copying "UnityEngine.dll" should be sufficient.

## License
Copyright (c) 2019 Enes Sadık Özbek. All rights reserved. 

Everything under "ImperialStudio" namespace is licensed as Creative Commons Attribution-NonCommercial-ShareAlike 4.0. See LICENSE file for the full license text.

In summary; 
You are free to:
* Share — copy and redistribute the material in any medium or format
* Adapt — You can remix, transform, and build upon the material

Under the following terms:
* Attribution — You must give appropriate credit, provide a link to the license, and indicate if changes were made. You may do so in any reasonable manner, but not in any way that suggests the licensor endorses you or your use.
* ShareAlike — If you remix, transform, or build upon the material, you must distribute your contributions under the same license as the original.
* No additional restrictions — You may not apply legal terms or technological measures that legally restrict others from doing anything the license permits.
* NonCommercial — You may not use the material for commercial purposes (contact es.ozbek [at] outlook.com for commercial licenses).