Allows modification of player data. To use, run the mod and place the following in the global settings file.
```
{
  "BoolData": {
    "fieldName_1": null,
    "fieldName_2": null,
    "fieldName_3": null
  },
  "IntData": [
    "intFieldName_1",
    "intFieldName_2"
  ]
}
```
where any number of fields is allowed. Then, each *bool field* can be set to true or false in the mod menu; 
while the mod is enabled, these bools will always return the provided value.

If DebugMod is installed, these toggles can also be bound to hotkeys. The "Save Overrides" option
will cause the values to persist, even when the mod is disabled. 

Debug also creates a panel that allows you to see the internal values of the listed fields. Use the 
Info Panel Switch function in the keybinds panel to view them.
