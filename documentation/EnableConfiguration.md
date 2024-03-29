﻿# Enabling **stylecop.json**

At this time, the code fix is not able to fully configure the newly-created **stylecop.json** file for use. This is
tracked in bug report [dotnet/roslyn#4655](https://github.com/dotnet/roslyn/issues/4655). In the mean time, users must
manually perform the following additional steps after creating the **stylecop.json** file.

In Visual Studio 2017, 2019, and 2022:

1. Select the file in **Solution Explorer**.
2. In the **Properties** window, set the value for **Build Action** to:
    * For most projects: **AdditionalFiles**
    * For projects using the new project system, including .NET Core and .NET Standard projects: **C# analyzer
      additional file**.

In Visual Studio 2015 Update 3 and newer:

1. Select the file in **Solution Explorer**.
2. In the Properties window set the value for **Build Action** to **AdditionalFiles**.

In older versions of Visual Studio 2015:

1. Right click the project in **Solution Explorer** and select **Unload Project**. If you are asked to save changes,
   click **Yes**.
2. Right click the unloaded project in **Solution Explorer** and select **Edit *ProjectName*.csproj**.
3. Locate the following item in the project file.

    ```csharp
    <None Include="stylecop.json" />
    ```

4. Change the definition to the following.

    ```csharp
    <AdditionalFiles Include="stylecop.json" />
    ```

5. Save and close the project file.
6. Right click the unloaded project in **Solution Explorer** and select **Reload Project**.

## Next steps

Additional information about the content of **stylecop.json** is available in [Configuration.md](Configuration.md).
