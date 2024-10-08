13
395136991233
19032507499434 1727844640369313000
{
  "name": "GravityCrushVFX",
  "local_enabled": true,
  "local_position": {

  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  }
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
  "cid": 3,
  "aoid": "19330443455799:1727844759734659500",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 1.5000000000000000,
    "is_trigger": true
  }
},
{
  "cid": 4,
  "aoid": "21358783114192:1727845572370604000",
  "component_type": "Mono_Component",
  "mono_component_type": "GravityFieldVFX",
  "data": {
    "EntityLifeTime": 5,
    "Animator": "19032508294642:1727844640369631800"
  }
},
{
  "cid": 2,
  "aoid": "54621832051966:1728365963231085100",
  "component_type": "Mono_Component",
  "mono_component_type": "GravityField",
  "data": {

  }
}
