11
210453397505
122396459949307 1720547509146683600
{
  "name": "Shield_FX",
  "local_enabled": true,
  "local_position": {
    "X": 0,
    "Y": 0
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  }
},
{
  "cid": 1,
  "aoid": "122396461017308:1720547509147188800",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/Shield/BAT003_shields.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0.5000000000000000,
    "skeleton_scale": {
      "X": 0.5000000000000000,
      "Y": 0.5000000000000000
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "143474948245860:1720557489486587500",
  "component_type": "Mono_Component",
  "mono_component_type": "ShieldVFX",
  "data": {
    "EntityLifeTime": 3,
    "Animator": "122396461017308:1720547509147188800"
  }
}
