# Povolení **stylecop.json**

V současnosti není kód oprav schopný plně nakonfigurovat nově vytvořený soubor **stylecop.json** pro použití. Toto je sledováno v hlášení o chybě [dotnet/roslyn#4655](https://github.com/dotnet/roslyn/issues/4655). Dokud nebude vyřešeno, musí uživatelé po vytvoření souboru **stylecop.json** manuálně provést následující dodatečné kroky.

V aplikaci Visual Studio 2017, 2019 a 2022:

1. Vyberte soubor v **Průzkumníku řešení**.
2. V okně **Vlastnosti** nastavte hodnotu **Akce sestavení** na:
    * Pro většinu projektů: **AdditionalFiles**
    * Pro projekty s novým projektem, včetně projektů .NET Core a .NET Standard: **C# analyzer additional file**.

V aplikaci Visual Studio 2015 Update 3 a novějším:

1. Vyberte soubor v **Průzkumníku řešení**.
2. V okně Vlastnosti nastavte hodnotu **Akce sestavení** na **AdditionalFiles**.

Ve starších verzích aplikace Visual Studio 2015:

1. Klikněte pravým tlačítkem myši na projekt v **Průzkumníku řešení** a vyberte **Unload Project**. Pokud se objeví otázka k uložení změn, klikněte na **Yes**.
2. Klikněte pravým tlačítkem myši na odpojený projekt v **Průzkumníku řešení** a vyberte **Edit *ProjectName*.csproj**.
3. Najděte následující položku v souboru projektu.

    ```csharp
    <None Include="stylecop.json" />
    ```

4. Změňte definici na následující.

    ```csharp
    <AdditionalFiles Include="stylecop.json" />
    ```

5. Uložte a zavřete soubor projektu.
6. Klikněte pravým tlačítkem myši na odpojený projekt v **Průzkumníku řešení** a vyberte **Reload Project**.

## Další kroky

Další informace o obsahu **stylecop.json** najdete v [Configuration.md](Configuration.md).
