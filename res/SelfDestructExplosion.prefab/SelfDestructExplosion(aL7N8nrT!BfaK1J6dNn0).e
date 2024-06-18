11
219043332097
115168708295379 1718733841037056500
{
  "name": "SelfDestructExplosion",
  "local_enabled": true,
  "local_position": {
    "X": 0,
    "Y": 0
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
  "aoid": "115204928294406:1718733858186672100",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/SelfDestructExplosion/explosion_big.spine",
    "ordered_skins": [
      "default"
    ],
    "depth_offset": 0,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    }
  }
},
{
  "cid": 2,
  "aoid": "116695728742282:1718734564057896600",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "StartAnimationStr": "Boom",
    "Loop": false,
    "Animator": "115204928294406:1718733858186672100"
  }
}
