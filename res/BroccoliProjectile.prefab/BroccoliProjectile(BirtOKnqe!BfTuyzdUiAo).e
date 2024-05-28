10
227633266689
434012051634846 1716921684324393000
{
  "name": "BroccoliProjectile",
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
  "sibling_index": 0
},
{
  "cid": 1,
  "aoid": "434012051769913:1716921684324456500",
  "component_type": "Internal_Component",
  "internal_component_type": "Sprite_Renderer",
  "data": {
    "texture": "projectile/broccoli.png",
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
  "cid": 5,
  "aoid": "439168799105689:1716924125967274300",
  "component_type": "Internal_Component",
  "internal_component_type": "Rigidbody",
  "data": {
    "angular_damping": 0,
    "linear_damping": 0,
    "gravity_scale": 1,
    "fixed_rotation": true
  }
},
{
  "cid": 4,
  "aoid": "439137855323351:1716924111315855700",
  "component_type": "Internal_Component",
  "internal_component_type": "Projectile",
  "data": {
    "speed": 15
  }
},
{
  "cid": 2,
  "aoid": "457738977608465:1716932918669342700",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 0.5000000000000000,
      "Y": 0.5000000000000000
    },
    "offset": {
      "X": 0,
      "Y": 0
    },
    "is_trigger": false,
    "density": 1,
    "friction": 0.2000000029802322,
    "restitution": 0,
    "restitution_threshold": 1
  }
}
