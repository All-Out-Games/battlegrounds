13
395136991233
115223630534342 1726551879408525800
{
  "name": "Crate",
  "local_enabled": true,
  "local_position": {
    "X": -15.5671501159667969,
    "Y": -17.0191230773925781
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
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
  "cid": 2,
  "aoid": "15864820519278:1726807060608608600",
  "component_type": "Mono_Component",
  "mono_component_type": "Crate",
  "data": {
    "Animator": "115223630726968:1726551879408602400",
    "HitPoint": 1,
    "Fade": "15884528796104:1726807068504560100"
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
  "cid": 5,
  "aoid": "17928165702960:1726807887270098100",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 0.8056840896606445,
      "Y": 0.4459838867187500
    },
    "offset": {
      "X": 0.0037531852722168,
      "Y": 0.2194652557373047
    }
  }
}
