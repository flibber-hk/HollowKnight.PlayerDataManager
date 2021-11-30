Allows modification of player data. To use, run the mod and place the following in the global settings file.
```
{
  "BoolData": {
    "fieldName_1": null,
    "fieldName_2": null,
    "fieldName_3": null
  }
}
```
where any number of fields is allowed. Then, each can be set to true or false in the mod menu; 
while the mod is enabled, these bools will always return the provided value.

If DebugMod is installed, these toggles can also be bound to hotkeys. The "Save Overrides" option
will cause the values to persist, even when the mod is disabled. 
