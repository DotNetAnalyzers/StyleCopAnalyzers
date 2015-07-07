Analyzer Tutorial
=================

This tutorial is going to guide you to write a diagnostic analyzer that enforces the placement of one 
space between the if keyword of an if statement and the open parenthesis of the condition.
 
This tutorial itself was written as an analyzer, meaning that the instructions of the tutorial are 
presented using the error underlines and light bulb code fixes in Visual Studio.

Analyzer Information
--------------------

Analyzers are tools that produce live warnings in Visual Studio to dynamically highlight potential 
problems in the code.
An analyzer operates by examining the syntax tree and semantic model describing the corresponding code. 
The information in the syntax tree and semantic model is built up during compilation, and is exposed by 
the .NET Compiler Platform (aka Roslyn). An analyzer is triggered when changes are made to the syntax 
tree, and then the analyzer can walk the syntax tree, looking at syntax nodes, syntax tokens and syntax 
trivia, and decide whether or not to surface a diagnostic. 

You can explore syntax trees with the Roslyn Syntax Visualizer. An example diagram of the syntax tree of
an if statement can be found below.
![If Statement Syntax Tree](https://github.com/dotnet/roslyn-analyzers/blob/master/IfSyntaxTree.jpg)

More Information
----------------

More information about Roslyn and the Roslyn source code can be found at https://github.com/dotnet/roslyn
More information on syntax trees can be found at 
http://blogs.msdn.com/b/csharpfaq/archive/2014/04/17/visualizing-roslyn-syntax-trees.aspx
More information on the Roslyn API can be found at http://www.coderesx.com/roslyn/html/Welcome.htm

Instructions
------------

Before you begin, go to Tools->Extensions and Updates->Online, and install:
	* .NET Compiler SDK
	* Roslyn Syntax Visualizer

Then, once you've re-started Visual Studio:
	* Open the DiagnosticAnalyzer.cs file
	* Open the Solution Explorer, right click on the .Vsix project, click "Set as StartUp project"
	* Go to View->Other Windows->Roslyn Syntax Visualizer. This will open the syntax visualizer,
          displaying the syntax tree of your code.
	  This will be useful when you are writing your analyzer, as the thing that you are analyzing is
          the syntax tree.
	* You will notice that something is underlined in red, and there is a corresponding error in the
          error list
          Errors directly related to this tutorial will begin with 'T:'
	* Read the error and try to fix the problem
	* If you are stuck, hover over the underline and a light bulb will show up. Clicking on this 
          lightbulb will bring up a list of code fixes.
	  Code fixes directly related to this tutorial will begin with 'T:' and clicking 
          on the code fix will add the code for whatever you were stuck on.
	* Continue to follow these error messages and code fixes to the end of the tutorial
	* Congratulations! You have written an analyzer!

If you would like to try your analyzer out, or if you would like to debug at any step of the tutorial, 
make sure you are in debug mode and press F5. This will launch a second instance of Visual Studio with 
your analyzer deployed as a VSIX (Visual Studio Extension). You can then open a new file and any messages 
from your analyzer will appear.

Packaging
---------

Building this project will produce an analyzer .dll (dynamic-link library), as well as the
following two ways you may wish to package that analyzer:
	 * A NuGet package (.nupkg file) that will add your assembly as a
	   project-local analyzer that participates in builds.
	 * A VSIX extension (.vsix file) that will apply your analyzer to all projects
	   and works just in the IDE.

TRYING OUT YOUR NUGET PACKAGE

To try out the NuGet package:
1. Create a local NuGet feed by following the instructions here:
	 > http://docs.nuget.org/docs/creating-packages/hosting-your-own-nuget-feeds
2. Copy the .nupkg file into that folder.
3. Open the target project in Visual Studio 2015.
4. Right-click on the project node in Solution Explorer and choose Manage
	 NuGet Packages.
5. Select the NuGet feed you created on the left.
6. Choose your analyzer from the list and click Install.

If you want to automatically deploy the .nupkg file to the local feed folder
when you build this project, follow these steps:
1. Right-click on this project in Solution Explorer and choose Properties.
2. Go to the Compile tab.
3. Click the Build Events button.
4. In the "Post-build event command line" box, change the -OutputDirectory path to point to your 
   local NuGet feed folder.