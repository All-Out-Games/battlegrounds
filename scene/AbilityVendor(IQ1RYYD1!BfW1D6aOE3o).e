11
227633266689
36341083635957 1717793673228865000
{
  "name": "AbilityVendor",
  "local_enabled": true,
  "local_position": {
    "X": -15.0361022949218750,
    "Y": 0.4349762201309204
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "previous_sibling": "31069919089008:1716413396906995300",
  "next_sibling": "36794150615657:1717793854746041500",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "36341083804784:1717793673228931800",
  "component_type": "Internal_Component",
  "internal_component_type": "Sprite_Renderer",
  "data": {
    "texture": "environment/CentralHub/Booth.png",
    "depth_offset": 0,
    "tint": {
      "X": 1,
      "Y": 1,
      "Z": 1,
      "W": 1
    },
    "layer": 0,
    "wait_for_load": false,
    "wrap": false,
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "36991757912748:1717793933915613200",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "prompt_offset": {
      "X": 0,
      "Y": 1
    },
    "text": "Ability Vendor",
    "hold_text": "",
    "radius": 2,
    "required_hold_time": 0.6000000238418579
  }
},
{
  "cid": 3,
  "aoid": "37006873513662:1717793939971541500",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 3,
    "offset": {
      "X": 0,
      "Y": 0
    },
    "is_trigger": true,
    "density": 1,
    "friction": 0.2000000029802322,
    "restitution": 0,
    "restitution_threshold": 1
  }
},
{
  "cid": 4,
  "aoid": "37153518500894:1717793998723526200",
  "component_type": "Mono_Component",
  "mono_component_type": "UniqueWindowInteractable",
  "data": {
    "WindowPrefabPath": "AbilityVendorMenuWindow.prefab"
  }
}
