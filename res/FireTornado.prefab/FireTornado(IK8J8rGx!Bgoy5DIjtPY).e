13
322122547201
35936158265777 1740865078679229400
{
  "name": "FireTornado",
  "local_enabled": true,
  "local_position": {

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
  "aoid": "35936158519886:1740865078679330700",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/FireTornado/fire_tornado.spine",
    "ordered_skins": [

    ]
  }
},
{
  "cid": 2,
  "aoid": "36053589740799:1740865125727074200",
  "component_type": "Internal_Component",
  "internal_component_type": "Projectile",
  "data": {

  }
},
{
  "cid": 3,
  "aoid": "36076365652180:1740865134852034400",
  "component_type": "Mono_Component",
  "mono_component_type": "FireTornadoProjectile",
  "data": {

  }
},
{
  "cid": 4,
  "aoid": "36088134537475:1740865139567130600",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 1.2000000476837158,
    "offset": {
      "Y": 0.2500000000000000
    },
    "is_trigger": true
  }
},
{
  "cid": 5,
  "aoid": "36242154135083:1740865201273671700",
  "component_type": "Internal_Component",
  "internal_component_type": "Rigidbody",
  "data": {
    "fixed_rotation": true
  }
},
{
  "cid": 6,
  "aoid": "41854451533493:1740867449789216900",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "PersistTime": 10,
    "FadeTime": 10.5000000000000000
  }
}
