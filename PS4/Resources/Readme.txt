Kedar Bastakoti
University of Utah
Fall 2015, CS3500


Important Notes on Tests:
Some of the tests need pre-written XML files that are saved in the PS4\SpreadsheetTests\bin\Debug directory.

PS5 Branch
A new branch from the existing PS4 solution was forked and named as PS5.

It will use built in C# XML libraries to handle reading and writing of XML documents.

New private codes will be added such that it will still be compatible with the old libraries developed before PS4.

To make the calculations faster the way to calculate Value of a Cell is updated. Whenever a cell is changed, 
all it's dependees(PS1 naming convention) will be updated instantly. Invalid formulas will simply contain 
FormulaError objects. Using this approach, whenever a value needs to be checked, we simply look at the Value 
property of the Cell, which is much faster than having to calculate every time it is queried. This also makes
lengthy or recursive kind of formulas operate faster, say for example a Fibonachi style cell assignments/formulas.

A through testings and critical optimizations were performed to make sure the application performs within acceptable
constraints. Since, an usable spreadsheet will have at least about 30 x 30 cells used, the Dictionary object will be
allocated 900 spaces for cells. Doing this will improve performance, since, it doesn't have to re-allocate memory each
time a new cell is added.


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

