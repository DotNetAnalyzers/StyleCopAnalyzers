# 启用 **stylecop.json**

目前，代码修复无法完整配置新建的 **stylecop.json** 文件，使其能够直接使用。
此问题记录在缺陷报告 [dotnet/roslyn#4655](https://github.com/dotnet/roslyn/issues/4655) 中。
在问题解决之前，用户必须在创建 **stylecop.json** 文件后手动完成以下额外步骤。

在 Visual Studio 2017、2019 和 2022 中：

1. 在**解决方案资源管理器**中选择该文件。
2. 在**属性**窗口中，将**生成操作**的值设置为：
    * 大多数项目：**AdditionalFiles**
    * 使用新项目系统的项目（包括 .NET Core 和 .NET Standard 项目）：**C# 分析器附加文件**。

在 Visual Studio 2015 Update 3 及更新版本中：

1. 在**解决方案资源管理器**中选择该文件。
2. 在**属性**窗口中，将**生成操作**的值设置为 **AdditionalFiles**。

在较早版本的 Visual Studio 2015 中：

1. 在**解决方案资源管理器**中右键单击项目，然后选择**卸载项目**。
   如果系统询问是否保存更改，请单击**是**。
2. 在**解决方案资源管理器**中右键单击已卸载的项目，然后选择**编辑 *ProjectName*.csproj**。
3. 在项目文件中找到以下项。

    ```csharp
    <None Include="stylecop.json" />
    ```

4. 将其定义更改为以下内容。

    ```csharp
    <AdditionalFiles Include="stylecop.json" />
    ```

5. 保存并关闭项目文件。
6. 在**解决方案资源管理器**中右键单击已卸载的项目，然后选择**重新加载项目**。

## 后续步骤

有关 **stylecop.json** 内容的其他信息，请参阅 [Configuration.md](Configuration.md)。
