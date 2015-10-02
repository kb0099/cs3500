Kedar Bastakoti
University of Utah
Fall 2015, CS3500
09/30/2015

PS4 Implementation

Initial Thoughts/Concerns: First of all, when deriving the 
class Spreadsheet from AbstractSpreadsheet, eventhough the XML
comments are present on the parent class, derived class still 
needed to repeat the XML comments. In fact, it has been found 
that there exist certain external tools which can create XML 
comments for derived class based upon the parent class. It is
indicated that Java has built in symantics for comment
inheritance. However, in C# tools like GhostDoc and SandCastle 
can do the same job!

In this small project, however, repeating the comments in the base
and derived classes should not have that much negative impact.

After reading the specifications, some black box testing methods 
were created, which helped to better understand some hidden intricacies
in the project specifications and requirements. 

A private cell class was created to help avoid clutter. A dictionary
of string name to Cell mapping was choosen, as this was easy to implement
and played nicely with the other interfaces and required less amount of code.

Previous versions of PS3 and PS2 dll files were copied to Resources/Libraries,
and referenced by the project and test project. As more methods were added
tests were ran and errors/bugs were fixed/debugged. Currently, all of the
projects and dlls are based upon .Net framework 4.6.

As changes were made, they were frequently committed and synchronized to the 
githup repository. Here is a small log of changes that relate to PS4:

0815d15 PS4: GetNamesOfAllNonemptyCells() ignores the empty cells
8acfde9 PS4: Makes adjustments to Tests after debug
636fd58 PS4: Updates the SetContentsHelper() to include Formula object
84460b5 PS4: Updates GetDirectDependents()
30c3fed PS4: Updates the private Cell class
8266e62 PS4: Adds more blackbox tests in SpreadsheetTests.cs
005c1d5 PS4: Changes settings for PS4.sln and Test project
f667b90 PS4: Adds a class comment for SpreadsheetTests
05e89cd PS4: Updates project settings for SpreadsheetTests
65d62b6 PS4: Adds refrences to the PS4/Resources/Libraries
d338247 PS4: Adds comments to the Test methods
8c3f19c PS4: Updates SpreadsheetTests.cs with BlackBox tests
9dff77c PS4: Adds comments for Visit method on AbstractSpreadsheet
ee66b69 PS4: Adds comments on derived class
268b166 PS4: Updates the Readme.txt
c42403a PS4: Adds Resources project
2507ecf Merge branch 'master' of https://github.com/uofu-cs3500-fall15/00574564
4765c74 PS4: Create and establish base requirements for PS4
368d8fc Updates Equals method and tests for it
831ff17 PS3: Updates GetHashCoode() and Tests for GetHashCode()

