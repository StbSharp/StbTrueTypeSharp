# StbTrueTypeSharp
[![NuGet](https://img.shields.io/nuget/v/StbTrueTypeSharp.svg)](https://www.nuget.org/packages/StbTrueTypeSharp/) [![Build status](https://ci.appveyor.com/api/projects/status/isyfkbfakrhoa1bm?svg=true)](https://ci.appveyor.com/project/RomanShapiro/stbtruetypesharp)

C# port of stb_truetype.h

# Adding Reference
There are two ways of referencing StbTrueTypeSharp in the project:
1. Through nuget: `install-package StbTrueTypeSharp`
2. As submodule:
    
    a. `git submodule add https://github.com/StbSharp/StbTrueTypeSharp.git`
    
    b. Now there are two options:
       
      * Add StbTrueTypeSharp/src/StbTrueTypeSharp/StbTrueTypeSharp.csproj to the solution
       
      * Include *.cs from StbTrueTypeSharp/src/StbTrueTypeSharp directly in the project. In this case, it might make sense to add STBSHARP_INTERNAL build compilation symbol to the project, so StbTrueTypeSharp classes would become internal.

# License
Public Domain

# Who uses it?
https://github.com/rds1983/SpriteFontPlus

## Credits
* [stb](https://github.com/nothings/stb)
