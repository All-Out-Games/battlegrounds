11
219043332097
120771259104021 1718736493759224700
{
  "name": "SelfDestructExplosion",
  "local_enabled": true,
  "local_position": {
    "X": 0,
    "Y": 0
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.5000000000000000,
    "Y": 0.5000000000000000
  },
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "120771260050594:1718736493759672800",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/SelfDestructExplosion/explosion_big.spine",
    "ordered_skins": [
      "default"
    ],
    "depth_offset": 0,
    "skeleton_scale": {
      "X": 0.5000000000000000,
      "Y": 0.5000000000000000
    }
  }
},
{
  "cid": 2,
  "aoid": "120984462125624:1718736594707596400",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "StartAnimationStr": [
      "Boom",
      "Kaboom"
    ],
    "Loop": false,
    "Animator": "120771260050594:1718736493759672800",
    "EntityLifeTime": 2.5000000000000000
  }
}
