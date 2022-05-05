Allows modification of player data. To use, run the mod and place the following in the global settings file.
```
{
  "BoolData": {
    "fieldName_1": null,
    "fieldName_2": null,
    "fieldName_3": null
  },
  "IntData": [
    {
      "intName": "intFieldName_1",
      "values": null,
      "current": null
    }
    {
      "intName": "intFieldName_2",
      "values": [
        1,
        3,
        6
      ],
      "current": null
    }
  ]
}
```
where any number of fields for bools and ints are allowed. 

* BoolData must be a dictionary "fieldName": currentValue, where currentValue is `true`, `false` or `null` for
"always true", "always false" and "not overridden" respectively.

* IntData must be a list of objects of the form above. The intName field gives the name of the pd int to override. The values field is a list of ints that can
be the value of the player data int. The current value gives the value of the pd int, which must be an entry from the values list - null for not overridden.
The values list can also be null - in this case, it will not be possible to override the pd int (but it can be monitored in the debug info panel).

Each field (except for non-modifiable int fields) can be modified by selecting the relevant option in the mod menu. 
The values will not be saved when the mod is uninstalled.

If DebugMod is installed, these toggles can also be bound to hotkeys. The "Save Overrides" option will cause the values to persist, 
even when the mod is uninstalled. 

Debug also creates a panel that allows you to see the internal values of the listed fields. Use the 
Info Panel Switch function in the debug mod keybinds panel to view them.
