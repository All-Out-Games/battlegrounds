13
395136991233
19032507499434 1727844640369313000
{
  "name": "GravityCrushVFX",
  "local_enabled": true,
  "local_position": {
    "X": -16.7757415771484375,
    "Y": -17.5321102142333984
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "previous_sibling": "54040865964219:1723590134132524500"
},
{
  "cid": 1,
  "aoid": "19032508294642:1727844640369631800",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/GravityCrush/BAT003_gravity_field_AOE.spine",
    "ordered_skins": [
      "blue_sparkles"
    ]
  }
},
{
  "cid": 2,
  "aoid": "19124973574099:1727844677415009400",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": true,
    "Animator": "19032508294642:1727844640369631800",
    "EntityLifeTime": 50,
    "StartAnimationStr": [
      "loop"
    ]
  }
},
{
  "cid": 3,
  "aoid": "19330443455799:1727844759734659500",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 1.5000000000000000
  }
}
