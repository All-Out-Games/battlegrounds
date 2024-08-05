13
42949672961
20565399938462 1722884878392140500
{
  "name": "Stat_Aura",
  "local_enabled": true,
  "local_position": {
    "X": 0.3781853616237640,
    "Y": -1.5056064128875732
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  }
},
{
  "cid": 1,
  "aoid": "20565401109572:1722884878392609000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/StatusUp/BAT003_status_up.spine",
    "ordered_skins": [
      "attack",
      "defense",
      "speed",
      "health"
    ],
    "depth_offset": 1.7799999713897705,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 3,
  "aoid": "28466368420340:1722888043843632900",
  "component_type": "Mono_Component",
  "mono_component_type": "StatAuraVFX",
  "data": {
    "EntityLifeTime": 20,
    "Animator": "20565401109572:1722884878392609000"
  }
}
