# TTToolbar

This app was created in order to have a central location where in-house apps/web pages can be launched from.

The shortcut info is read in from an xml file (which has been excluded due to it being of a sensitive nature).

An example of an xml file:
```xml
<ToolbarShortcuts>
  <!-- Exe -->
  <ToolbarShortcut name="shortcutName1" iconFilename="Images\\icon1.png" exeRelFilename="specify relative path to exe"/>
  
  <!-- Web address -->
  <ToolbarShortcut name="shortcutName2" iconFilename="Images\\icon2.png" link="specify web address"/>
</ToolbarShortcuts>
```
Notes:

1. Current location is written to registry on exit, and read from registry to position form on startup.
2. Icons can be arranged according to preference. The arrangement is written to registry on exit, and reloaded to arrange icons on startup.
3. Shortcut names must be unique.
