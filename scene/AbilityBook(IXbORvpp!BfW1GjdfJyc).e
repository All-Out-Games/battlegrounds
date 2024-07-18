11
223338299393
36794150615657 1717793854746041500
{
  "name": "AbilityBook",
  "local_enabled": true,
  "local_position": {
    "X": 0.5889350175857544,
    "Y": -5.3936033248901367
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "previous_sibling": "36341083635957:1717793673228865000",
  "next_sibling": "26663308607822:1721081450537125700",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "36794150733654:1717793854746088300",
  "component_type": "Internal_Component",
  "internal_component_type": "Sprite_Renderer",
  "data": {
    "texture": "environment/CentralHub/HallShelf.png",
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
  "aoid": "38825394009811:1717794668545287700",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 1,
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
  "cid": 3,
  "aoid": "38838298168786:1717794673715222200",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "prompt_offset": {
      "X": 0,
      "Y": 1
    },
    "text": "Ability Book",
    "hold_text": "",
    "radius": 2,
    "required_hold_time": 0.6000000238418579
  }
},
{
  "cid": 4,
  "aoid": "38861390310195:1717794682966879100",
  "component_type": "Mono_Component",
  "mono_component_type": "UniqueWindowInteractable",
  "data": {
    "WindowPrefabPath": "AbilityBook.prefab"
  }
}
