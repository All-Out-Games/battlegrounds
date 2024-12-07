13
304942678017
115223630534342 1726551879408525800
{
  "name": "AdCrab",
  "local_enabled": true,
  "local_position": {
    "X": -7.3849744796752930,
    "Y": -16.8422393798828125
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "network_position": true
},
{
  "cid": 1,
  "aoid": "115223630726968:1726551879408602400",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "Props/Crab/crab.spine",
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
  "cid": 4,
  "aoid": "780105956615190:1732923285749067800",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "required_hold_time": 0.3000000119209290
  }
},
{
  "cid": 2,
  "aoid": "55167987182250:1733556848726986800",
  "component_type": "Mono_Component",
  "mono_component_type": "AdCrab",
  "data": {
    "Trigger": "780105956615190:1732923285749067800",
    "Animator": "115223630726968:1726551879408602400",
    "Fade": "15884528796104:1726807068504560100"
  }
}
