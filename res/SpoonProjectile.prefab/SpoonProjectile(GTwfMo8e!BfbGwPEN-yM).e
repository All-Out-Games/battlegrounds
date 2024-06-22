11
219043332097
27746012139294 1718997385762831500
{
  "name": "SpoonProjectile",
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
  "aoid": "27766904744318:1718997394133269100",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "projectile/BAT003_Projectile/BAT003_projectile.spine",
    "ordered_skins": [
      "default",
      "spoon"
    ],
    "depth_offset": -0.5000000000000000,
    "skeleton_scale": {
      "X": 0.5000000000000000,
      "Y": 0.5000000000000000
    }
  }
},
{
  "cid": 2,
  "aoid": "28134790163612:1718997541523309800",
  "component_type": "Internal_Component",
  "internal_component_type": "Rigidbody",
  "data": {
    "angular_damping": 0,
    "linear_damping": 0,
    "gravity_scale": 1,
    "fixed_rotation": false
  }
},
{
  "cid": 3,
  "aoid": "28146154641459:1718997546076387600",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 1,
      "Y": 1
    },
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
  "cid": 4,
  "aoid": "28175650223182:1718997557893531700",
  "component_type": "Mono_Component",
  "mono_component_type": "SpoonProjectile",
  "data": {
    "Damage": 0,
    "Pierce": false,
    "Owner": "0:0"
  }
},
{
  "cid": 5,
  "aoid": "28193434602343:1718997565018685900",
  "component_type": "Internal_Component",
  "internal_component_type": "Projectile",
  "data": {
    "speed": 0,
    "direction": {
      "X": 0,
      "Y": 0
    },
    "start_position": {
      "X": 0,
      "Y": 0
    }
  }
}
