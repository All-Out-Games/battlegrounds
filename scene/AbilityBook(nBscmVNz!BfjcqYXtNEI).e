11
115964116994
171640257860467 1721345540273525000
{
  "name": "AbilityBook",
  "local_enabled": true,
  "local_position": {
    "X": -11.0700988769531250,
    "Y": 3.2310636043548584
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 2.5000000000000000,
    "Y": 2.5000000000000000
  },
  "previous_sibling": "36322976093087:1721085320598029900",
  "next_sibling": "68101061239733:1721941650834589300",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "171640261391599:1721345540275196500",
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
  "aoid": "171640267323995:1721345540278005400",
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
  "aoid": "171640271786328:1721345540280118200",
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
  "aoid": "171640276285123:1721345540282248100",
  "component_type": "Mono_Component",
  "mono_component_type": "UniqueWindowInteractable",
  "data": {
    "WindowPrefabPath": "AbilityLoadoutPage.prefab"
  }
}
