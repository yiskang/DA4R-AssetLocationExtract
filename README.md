# Extract MEP Asset Location from RVT

[![Design-Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)

![Windows](https://img.shields.io/badge/Plugins-Windows-lightgrey.svg)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)
[![Revit-2022](https://img.shields.io/badge/Revit-2022-lightgrey.svg)](http://autodesk.com/revit)

## Description

This addin will extract MEP elements basic info by where the room they belong to. (A.K.A. Finding MEP elements by rooms)

## Example Result in JSON

```json
{
  "id": "e1d4bdfb-2688-47c1-a386-3b48116e5fd2-0008c15d",
  "category": "Spaces",
  "type": "Room",
  "level": "Level 1",
  "name": "CH/PH Stock 1",
  "children": [
    {
      "id": "e16114b1-3e00-4772-899a-854cadacc959-0006dd81",
      "category": "Assets",
      "type": "Lighting Devices",
      "name": "Passive Infrared - 277 V",
      "children": []
    },
    {
      "id": "4403b060-3f13-475e-a17a-0a0bc88cc65c-00071729",
      "category": "Assets",
      "type": "Electrical Fixtures",
      "name": "Standard",
      "children": []
    },
    {
      "id": "4403b060-3f13-475e-a17a-0a0bc88cc65c-00071765",
      "category": "Assets",
      "type": "Electrical Fixtures",
      "name": "Standard",
      "children": []
    },
    {
      "id": "4403b060-3f13-475e-a17a-0a0bc88cc65c-000717c2",
      "category": "Assets",
      "type": "Electrical Fixtures",
      "name": "Standard",
      "children": []
    },
    {
      "id": "4403b060-3f13-475e-a17a-0a0bc88cc65c-000717f2",
      "category": "Assets",
      "type": "Electrical Fixtures",
      "name": "Standard",
      "children": []
    },
    {
      "id": "4403b060-3f13-475e-a17a-0a0bc88cc65c-0007188c",
      "category": "Assets",
      "type": "Electrical Fixtures",
      "name": "Standard",
      "children": []
    },
    {
      "id": "4403b060-3f13-475e-a17a-0a0bc88cc65c-000718ac",
      "category": "Assets",
      "type": "Electrical Fixtures",
      "name": "Standard",
      "children": []
    }
  ]
}
```

## Forge DA Setup

### Activity via [POST activities](https://forge.autodesk.com/en/docs/design-automation/v3/reference/http/activities-POST/)

```json
{
    "commandLine": [
        "$(engine.path)\\\\revitcoreconsole.exe /i \"$(args[inputFile].path)\" /al \"$(appbundles[AssetLocationExtract].path)\""
    ],
    "parameters": {
        "inputFile": {
            "verb": "get",
            "description": "Input Revit File",
            "required": true,
            "localName": "$(inputFile)"
        },
        "result": {
            "zip": false,
            "verb": "put",
            "description": "Result JSON File",
            "required": true,
            "localName": "result.json"
        }
    },
    "id": "youralais.AssetLocationExtractActivity+dev",
    "engine": "Autodesk.Revit+2022",
    "appbundles": [
        "youralais.AssetLocationExtract+dev"
    ],
    "settings": {},
    "description": "Activity of extracting MEP Asset Location Info from RVT",
    "version": 1
}
```

### Workitem via [POST workitems](https://forge.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-POST/)

```json
{
    "activityId": "youralais.AssetLocationExtractActivity+dev",
    "arguments": {
      "inputFile": {
        "verb": "get",
        "url": "https://developer.api.autodesk.com/oss/v2/signedresources/...region=US"
      },
      "result": {
        "verb": "put",
        "url": "https://developer.api.autodesk.com/oss/v2/signedresources/...?region=US"
      }
    }
}
```

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Written by

Eason Kang [@yiskang](https://twitter.com/yiskang), [Forge Partner Development](http://forge.autodesk.com)
