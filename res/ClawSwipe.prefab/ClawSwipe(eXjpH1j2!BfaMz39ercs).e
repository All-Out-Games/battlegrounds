11
219043332097
133560214182134 1718742549133965100
{
  "name": "ClawSwipe",
  "local_enabled": true,
  "local_position": {
    "X": 2.4460542201995850,
    "Y": -3.9565215110778809
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
  "aoid": "133560215032735:1718742549134367300",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/ClawSwipe/claw_swipe_BAT003.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "133592911862336:1718742564615816900",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": false,
    "Animator": "133560215032735:1718742549134367300",
    "EntityLifeTime": 2,
    "StartAnimationStr": [
      "slice"
    ]
  }
}
