13
296352743425
59804924382647 1719617016939560100
{
  "name": "ChargingStation",
  "local_enabled": true,
  "local_position": {
    "X": 1.7339445352554321,
    "Y": -6.4291543960571289
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "next_sibling": "377316458197998:1716489394196426200",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "59804925535505:1719617016940021600",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "Props/LightningTrap/BAT003_lightning_trap.spine",
    "ordered_skins": [

    ],
    "skeleton_scale": {
      "X": 1.2000000476837158,
      "Y": 1.2000000476837158
    }
  }
},
{
  "cid": 2,
  "aoid": "59827397374439:1719617025943161600",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 0.7500000000000000,
    "is_trigger": true
  }
},
{
  "cid": 3,
  "aoid": "998938446028282:1733527510716900500",
  "component_type": "Mono_Component",
  "mono_component_type": "ChargingStation",
  "data": {
    "TriggerCollider": "59827397374439:1719617025943161600",
    "Animator": "59804925535505:1719617016940021600"
  }
}
