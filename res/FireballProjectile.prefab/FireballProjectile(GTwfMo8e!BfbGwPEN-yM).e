13
219043332097
27746012139294 1718997385762831500
{
  "name": "FireballProjectile",
  "local_enabled": true,
  "local_position": {
    "X": 0.0000000596046448
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
  "aoid": "27766904744318:1718997394133269100",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/FireBall/BAT003_fireball.spine",
    "ordered_skins": [
      "default"
    ],
    "depth_offset": -0.2431195378303528,
    "skeleton_scale": {
      "X": 1.2000000476837158,
      "Y": 1.2000000476837158
    }
  }
},
{
  "cid": 2,
  "aoid": "28134790163612:1718997541523309800",
  "component_type": "Internal_Component",
  "internal_component_type": "Rigidbody",
  "data": {

  }
},
{
  "cid": 3,
  "aoid": "28146154641459:1718997546076387600",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 1.9989795684814453,
      "Y": 1.6850147247314453
    },
    "offset": {
      "X": -0.2283385992050171,
      "Y": 0.6136599779129028
    },
    "is_trigger": true
  }
},
{
  "cid": 5,
  "aoid": "28193434602343:1718997565018685900",
  "component_type": "Internal_Component",
  "internal_component_type": "Projectile",
  "data": {

  }
},
{
  "cid": 4,
  "aoid": "35274060899739:1731802022477593000",
  "component_type": "Mono_Component",
  "mono_component_type": "FireballProjectile",
  "data": {

  }
}
