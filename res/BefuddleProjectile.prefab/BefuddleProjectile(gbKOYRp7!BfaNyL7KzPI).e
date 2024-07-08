11
219043332098
142603892890235 1718746831177593800
{
  "name": "BefuddleProjectile",
  "local_enabled": true,
  "local_position": {
    "X": 0,
    "Y": 0
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.4999998211860657,
    "Y": 0.4999998211860657
  },
  "spawn_as_networked_entity": true
},
{
  "cid": 6,
  "aoid": "142742298702782:1718746896710629800",
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
  "cid": 2,
  "aoid": "142702165994579:1718746877708406200",
  "component_type": "Mono_Component",
  "mono_component_type": "BefuddleProjectile",
  "data": {
    "Damage": 0,
    "Pierce": false,
    "Owner": "0:0"
  }
},
{
  "cid": 4,
  "aoid": "142724977058611:1718746888509096300",
  "component_type": "Internal_Component",
  "internal_component_type": "Projectile",
  "data": {
    "speed": 15,
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
  "aoid": "142733776735698:1718746892675608600",
  "component_type": "Internal_Component",
  "internal_component_type": "Box_Collider",
  "data": {
    "size": {
      "X": 2.6126511096954346,
      "Y": 2.5642814636230469
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
  "cid": 1,
  "aoid": "45635518915896:1720471772593663800",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/ConfusionCloud/BAT003_confusion_cloud.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0.5000000000000000,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    }
  }
}
