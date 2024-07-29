11
223338299393
68101061239733 1721941650834589300
{
  "name": "AbilityVendor",
  "local_enabled": true,
  "local_position": {
    "X": -10.5577001571655273,
    "Y": 0.4349762201309204
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "previous_sibling": "171640257860467:1721345540273525000",
  "next_sibling": "198337835043157:1722286871474699800",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "68101072343721:1721941650840150000",
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
  "aoid": "68101077363539:1721941650842664100",
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
  "aoid": "68101081908439:1721941650844940100",
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
  "aoid": "68101086466133:1721941650847222500",
  "component_type": "Mono_Component",
  "mono_component_type": "UniqueWindowInteractable",
  "data": {
    "WindowPrefabPath": "SkillTreePage.prefab"
  }
}
