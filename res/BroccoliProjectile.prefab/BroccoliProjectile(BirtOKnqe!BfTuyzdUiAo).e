10
227633266689
434012051634846 1716921684324393000
{
  "name": "BroccoliProjectile",
  "local_enabled": true,
  "local_position": {
    "X": 4.7861504554748535,
    "Y": 2.5317230224609375
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "sibling_index": 0,
  "spawn_as_networked_entity": true,
  "network_id": 1
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
  "cid": 3,
  "aoid": "439072599203485:1716924080418058000",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 0.5400000214576721,
      "Y": 0.5600000023841858
    },
    "offset": {
      "X": 0,
      "Y": 0
    },
    "is_trigger": true,
    "density": 1,
    "friction": 0,
    "restitution": 0,
    "restitution_threshold": 1
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
  "cid": 5,
  "aoid": "439168799105689:1716924125967274300",
  "component_type": "Internal_Component",
  "internal_component_type": "Rigidbody",
  "data": {
    "angular_damping": 0,
    "linear_damping": 0,
    "gravity_scale": 0,
    "fixed_rotation": true
  }
}
