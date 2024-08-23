13
219043332097
15680058217161 1719340585550191700
{
  "name": "PsionicBeamExplosion",
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
  "aoid": "15680059245549:1719340585550678300",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/PsionicBeam/BAT003_psionic_explosion.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0.7500000000000000,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "15750387374007:1719340618849965200",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": false,
    "Animator": "0:0",
    "EntityLifeTime": 2,
    "IsPermanent": false,
    "StartAnimationStr": [
      "med_explosion",
      "small_explosion",
      "big_explosion",
      "og_explosion",
      "og_small_explosion"
    ]
  }
}
