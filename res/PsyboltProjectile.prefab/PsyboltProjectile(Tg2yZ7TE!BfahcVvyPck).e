11
219043332097
85820734682308 1718833290825758500
{
  "name": "PsyboltProjectile",
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
  "aoid": "85820735657166:1718833290826246600",
  "component_type": "Internal_Component",
  "internal_component_type": "Sprite_Renderer",
  "data": {
    "texture": "projectile/popcorn3.png",
    "depth_offset": 0,
    "tint": {
      "X": 1,
      "Y": 1,
      "Z": 1,
      "W": 1
    },
    "layer": 0,
    "wait_for_load": false,
    "wrap": false,
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "85847486295646:1718833304222999300",
  "component_type": "Mono_Component",
  "mono_component_type": "PsyBoltProjectile",
  "data": {
    "Damage": 0,
    "Pierce": false,
    "Owner": "0:0"
  }
},
{
  "cid": 3,
  "aoid": "85865375785689:1718833313182077900",
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
},
{
  "cid": 4,
  "aoid": "85916182835316:1718833338626310900",
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
  "cid": 5,
  "aoid": "86877723534317:1718833820167077100",
  "component_type": "Internal_Component",
  "internal_component_type": "Rigidbody",
  "data": {
    "angular_damping": 0,
    "linear_damping": 0,
    "gravity_scale": 1,
    "fixed_rotation": false
  }
}
