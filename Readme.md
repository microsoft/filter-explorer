Filter Explorer for Windows
===========================

Filter Explorer is a Nokia example application which demonstrates some of the image editing
capabilities and performance of the Nokia Imaging SDK by allowing the user to apply
a number of filter layers to existing photos.

This example application is hosted in GitHub:
https://github.com/nokia-developer/filter-explorer-rt/

Developed with Microsoft Visual Studio Express 2013 for Windows.

Compatible with:

 * Windows RT 8.1

Tested to work on:

 * Nokia Lumia 2520
 * Windows 8.1 x86 desktop


Instructions
------------

Make sure you have the following installed:

 * Windows 8.1
 * Visual Studio Express 2013 for Windows
 * Nuget 2.7 or later

To build and run the code sample in simulator:

 1. Open the SLN file:
    File > Open Project, select the solution (.sln postfix) file
 2. Select the target 'Simulator' and platform 'x86'.
 3. Press F5 to build the project and run it.

If the project does not compile on the first attempt it's possible that you
did not have the required packages yet. With Nuget 2.7 or later the missing
packages are fetched automatically when build process is invoked, so try
building again. If some packages cannot be found there should be an
error stating this in the Output panel in Visual Studio Express.

In addition to building and running from source, you can install
a pre-compiled test build package:

 1. Copy the "FilterExplorer_*_Test.zip" to the target device and uncompress it
 2. Go to folder "FilterExplorer_*_Test"
 3. Right click on file "Add-AppDevPackage.ps1" and select "Run with PowerShell"
 4. Read and accept all prompts (certificate installs etc.)
 5. Application should now be installed ("Filter Explorer")

For more information on deploying and testing applications see:
http://msdn.microsoft.com/library/windows/apps/hh441469.aspx


About the implementation
------------------------

| Folder | Description |
| ------ | ----------- |
| / | Contains the project file, the license information and this file (README.md) |
| FilterExplorer | Root folder for the implementation files.  |
| FilterExplorer/Assets | Graphic assets like icons and tiles. |
| FilterExplorer/Commands | MVVM commands. |
| FilterExplorer/Converters | XAML binding converters |
| FilterExplorer/Filters | Filter wrappers. |
| FilterExplorer/Models | MVVM models. |
| FilterExplorer/Strings | Localization files. |
| FilterExplorer/Utilities | Utility classes. |
| FilterExplorer/ViewModels | MVVM viewmodels. |
| FilterExplorer/Views | MVVM views. |
| FilterExplorer/Properties | Application property files. |

Important classes:

| Class | Description |
| ----- | ----------- |
| Filters.Filters | Imaging SDK filters are wrapped in application specific filters. |
| Models.FilteredPhotoModel | Contains PhotoModel and adds rendering loaded images with filters. |
| Models.PhotoModel | Handles reading image file contents. |
| Models.PhotoLibraryModel | Opens and saves files. |


Known issues
------------

None.


License
-------

See the license text file delivered with this project:
https://github.com/nokia-developer/filter-explorer-rt/blob/master/License.txt


Downloads
---------

| Project | Release | Download |
| ------- | --------| -------- |
| Filter Explorer for Windows | v0.2 | [filter-explorer-rt-0.2.zip](https://github.com/nokia-developer/filter-explorer-rt/archive/v0.2.zip) |


Version history
---------------

No public releases yet.
