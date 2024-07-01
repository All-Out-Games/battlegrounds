11
219043332097
59804924382647 1719617016939560100
{
  "name": "BearTrap",
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
  "aoid": "59804925535505:1719617016940021600",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "Props/BearTrap/BAT003_bear_trap.spine",
    "ordered_skins": [

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
  "aoid": "59827397374439:1719617025943161600",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 1.5000000000000000,
    "offset": {
      "X": 0,
      "Y": 0
    },
    "is_trigger": true,
    "density": 1,
    "friction": 0.2000000029802322,
    "restitution": 0,
    "restitution_threshold": 1
  }
},
{
  "cid": 3,
  "aoid": "59966660819584:1719617081737805000",
  "component_type": "Mono_Component",
  "mono_component_type": "BearTrap",
  "data": {
    "TriggerCollider": "59827397374439:1719617025943161600",
    "Animator": "59804925535505:1719617016940021600",
    "Snapped": false
  }
}
