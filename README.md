# GalleryAssignment
Gallery job assignment made in Unity 2021.2.8f1. Built version works on PC platforms (tested only on Windows and Linux).
Default path (which creates automatically on program launch): <PROJECT_PATH>/GalleryAssignment/GalleryAssignment_Data/Gallery

Features:
- Takes account of every recursive directory inside the main selected directory ("Gallery" as chosenDirectoryName variable in FileImageManager.cs),
- Works asynchronously (loads images in background, on different thread while the user can scroll and view others),
- Checks image by its image header not the extension (ImageHeaderChecker.cs),
- On start creates 3 placeholder images - bóbr.png, kosmos_0.png, kosmos_1.png. Remove Gallery folder to make it respawn.

[Scripts folder hierarchy](https://github.com/TheMatiaz0/GalleryAssignment/tree/main/Assets/Scripts): 
- Containers - contain data,
- Objects - can be initialized with parameters,
- RelatedUtility - related to the project, but are more useable in different cases than others,
- Utility - not related to the project (can be used everywhere).

What means "Modal"?
I took it from Bootstrap framework website definition: https://getbootstrap.com/docs/5.0/components/modal
It's a window on top of the main window. 

