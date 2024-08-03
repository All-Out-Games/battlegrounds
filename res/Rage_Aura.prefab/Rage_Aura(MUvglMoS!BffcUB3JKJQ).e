13
210453397506
54201960155666 1720218104369554000
{
  "name": "Rage_Aura",
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
  "aoid": "54201960338275:1720218104369626200",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/Rage/BAT003_rage_aura.spine",
    "ordered_skins": [

    ],
    "depth_offset": -5,
    "skeleton_scale": {
      "X": 1.2000000476837158,
      "Y": 1.2000000476837158
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "54245212971764:1720218121698413000",
  "component_type": "Mono_Component",
  "mono_component_type": "AttachmentObject",
  "data": {
    "EntityLifeTime": 15
  }
},
{
  "cid": 3,
  "aoid": "54300197286806:1720218143727394200",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": true,
    "Animator": "54201960338275:1720218104369626200",
    "EntityLifeTime": 9,
    "IsPermanent": false,
    "StartAnimationStr": [
      "rage_loop"
    ]
  }
},
{
  "cid": 4,
  "aoid": "55068734397186:1720218451635013400",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "FadeSprite": false,
    "PersistTime": 9,
    "FadeTime": 10
  }
}
