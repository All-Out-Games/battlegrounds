13
395136991233
115223630534342 1726551879408525800
{
  "name": "AdCrate",
  "local_enabled": true,
  "local_position": {
    "X": -16.6996288299560547,
    "Y": -17.3325672149658203
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "next_sibling": "377316458197998:1716489394196426200",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "115223630726968:1726551879408602400",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "Props/Crate/BAT003_crate.spine",
    "ordered_skins": [
      "default"
    ]
  }
},
{
  "cid": 5,
  "aoid": "17928165702960:1726807887270098100",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 1.1936597824096680,
      "Y": 1.2033252716064453
    },
    "offset": {
      "X": -0.0254797935485840,
      "Y": 0.3138017654418945
    },
    "is_trigger": true
  }
},
{
  "cid": 3,
  "aoid": "15884528796104:1726807068504560100",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "PersistTime": 10,
    "FadeTime": 11
  }
},
{
  "cid": 2,
  "aoid": "780095429331526:1732923280476989700",
  "component_type": "Mono_Component",
  "mono_component_type": "AdCrate",
  "data": {
    "Trigger": "780105956615190:1732923285749067800",
    "Animator": "115223630726968:1726551879408602400",
    "Fade": "15884528796104:1726807068504560100"
  }
},
{
  "cid": 4,
  "aoid": "780105956615190:1732923285749067800",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "required_hold_time": 0.3000000119209290
  }
}
