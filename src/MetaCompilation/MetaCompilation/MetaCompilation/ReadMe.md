Analyzer Tutorial
=================

This tutorial is going to guide you to write a diagnostic analyzer that enforces the placement of a single space between the `if` keyword of an if-statement and the opening parenthesis of the condition.
This tutorial itself was written as an analyzer, meaning that the instructions of the tutorial are presented using the error squiggles and light bulb code fixes in Visual Studio.

Analyzer Information
--------------------
Analyzers are tools that produce live diagnostics in Visual Studio, highlighting potential problems in the code. Diagnostics appear as squiggles under the incorrect code. Code Fixes appear as light bulbs with suggestions as to how to fix these errors.
An analyzer operates by examining the syntax tree representing the corresponding code.
The information in the syntax tree is built up during compilation, and is exposed by the .NET Compiler Platform (aka Roslyn). An analyzer is triggered when changes are made to the syntax tree, which happens when you edit a piece of code. The analyzer can then walk through the syntax tree, looking at syntax nodes, syntax tokens and syntax trivia, and decide whether or not to surface a diagnostic.
Analyzers can also examine the semantic model, a higher-level representation of the code, if the information required to surface a diagnostic cannot be obtained from the syntax tree.
Tutorial Overview
------------
Writing an analyzer can be broken down into the following high-level steps

1. Create an ID to distinguish the diagnostic from other diagnostics, as an analyzer can contain several diagnostics.
1. Create a diagnostic rule that is associated with the aforementioned ID and has properties such as a message to display to the user and whether the diagnostic is enabled by default.
1. Create an array to hold all diagnostics supported by your analyzer.
1. Use the Initialize method to identify a kind of syntax node (in this case if-statement syntax nodes) and call your analysis method.
1. Within your analysis method, navigate the syntax tree to determine if the spacing of the if-statement is correct. See the image below. This step is the main body of your analyzer.
1. If the analysis finds an error, create and report a diagnostic to inform the user of this error.

The syntax diagram for a sample if-statement can be found below. You will a write a diagnostic that will surface when the whitespace circled in red is either absent or anything other than a single space. 
For more information on visualizing syntax trees see [Syntax Trees](https://github.com/dotnet/roslyn-analyzers/blob/master/NewAnalyzerTemplate/NewAnalyzerTemplate/NewAnalyzerTemplate/README.md#syntax-trees).
![If Statement Syntax Tree](https://github.com/dotnet/roslyn-analyzers/blob/master/NewAnalyzerTemplate/NewAnalyzerTemplate/NewAnalyzerTemplate/IfSyntaxTree.jpg)

Instructions
------------
* Before you begin, go to *Tools -> Extensions and Updates -> Online* and install .NET Compiler Platform SDK.
* Restart Visual Studio.
* Open the DiagnosticAnalyzer.cs file
* Open the Solution Explorer, right click on the .Vsix project, click "Set as StartUp project"
* You will notice that something is squiggled in red, and there is a corresponding error in the error list. Errors directly related to this tutorial will begin with 'T':
	* Read the error and try to fix the problem
	* If you are stuck, hover over the squiggle and a light bulb will show up. Clicking on this lightbulb will bring up a list of code fixes.
	* Code fixes directly related to this tutorial will begin with 'T:' and clicking on the code fix will add the code for whatever you were stuck on.
* Continue to follow these error messages and code fixes to the end of the tutorial
* Congratulations! You have written an analyzer!

If you would like to try your analyzer out, or if you would like to debug at any step of the tutorial, make sure your configuration is set to Debug and press F5. This will launch a second instance of Visual Studio with your analyzer deployed as a VSIX (Visual Studio Extension). You can then open a new C# console app, write an if-statement, and any messages from your analyzer will appear.
If you would like to explore a code fix for your diagnostic, open up CodeFixProvider.cs. Edit the comment in FixableDiagnosticsId then press F5 again to see the code fix to go with your analyzer message.

Syntax Trees
------------
If you would like to see the syntax tree of your if-statement being analyzed you can follow the steps below.

* Go to *Tools -> Extensions and Updates -> Online*, and install Roslyn Syntax Visualizer.
* Restart Visual Studio.
* Again, you can open a new C# console app and write an if-statement.
* Go to *View -> Other Windows -> Roslyn Syntax Visualizer* to see the if-statement syntax tree.
* Right-click on any node and selected *View Directed Syntax Graph* to see a visual representation of the syntax tree starting at your selected node.

The Roslyn Syntax Visualizer is extremely useful when writing analyzers as it helps you determine exactly which syntax node you need to locate for a particular diagnostic.

More Information
----------------
Further information can be found by following the links below:
- [Roslyn overview](https://github.com/dotnet/roslyn/wiki/Roslyn%20Overview)
- [Roslyn source code](https://github.com/dotnet/roslyn)
- [Syntax trees](http://blogs.msdn.com/b/csharpfaq/archive/2014/04/17/visualizing-roslyn-syntax-trees.aspx)
- [Roslyn reference source](http://source.roslyn.codeplex.com/)
- Additional tutorials:
	- [Analyzer](https://msdn.microsoft.com/en-us/magazine/dn879356.aspx)
	- [Code Fix](https://msdn.microsoft.com/en-us/magazine/Dn904670.aspx)

Packaging
---------
Building this project will produce an analyzer .dll (dynamic-link library), as well as the following two ways you may wish to package that analyzer:

- A NuGet package (.nupkg file) that will add your assembly as a
	   project-local analyzer that participates in builds.
- A VSIX extension (.vsix file) that will apply your analyzer to all projects
	   and works just in the IDE.

#### Trying out your NuGet Package

To try out the NuGet package:

1. Create a local NuGet feed by following the instructions here: 
 	- http://docs.nuget.org/docs/creating-packages/hosting-your-own-nuget-feeds
1. Copy the .nupkg file into that folder.
1. Open the target project in Visual Studio 2015.
1. Right-click on the project node in Solution Explorer and choose Manage NuGet Packages.
1. Select the NuGet feed you created on the left.
1. Choose your analyzer from the list and click Install.

If you want to automatically deploy the .nupkg file to the local feed folder when you build this project, follow these steps:

1. Right-click on this project in Solution Explorer and choose Properties.
1. Go to the Compile tab.
1. Click the Build Events button.
1. In the "Post-build event command line" box, change the -OutputDirectory path to point to your 
   local NuGet feed folder.