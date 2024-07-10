11
219043332097
42374653696940 1719610033658776700
{
  "name": "ShurikenProjectile",
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
  "aoid": "42374655048414:1719610033659318000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "projectile/BAT003_Projectile/BAT003_projectile.spine",
    "ordered_skins": [
      "shuriken"
    ],
    "depth_offset": 0.5000000000000000,
    "skeleton_scale": {
      "X": 0.5000000000000000,
      "Y": 0.5000000000000000
    }
  }
},
{
  "cid": 2,
  "aoid": "42401681285807:1719610044487137200",
  "component_type": "Mono_Component",
  "mono_component_type": "ShurikenProjectile",
  "data": {
    "Damage": 0,
    "Pierce": false,
    "Owner": "0:0"
  }
},
{
  "cid": 3,
  "aoid": "42425838720140:1719610054165594400",
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
  "cid": 4,
  "aoid": "42437895632843:1719610058996087800",
  "component_type": "Internal_Component",
  "internal_component_type": "Projectile",
  "data": {
    "speed": 10,
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
  "cid": 5,
  "aoid": "42483072584644:1719610077095826000",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 1,
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
}
