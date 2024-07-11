11
206158430209
238435753726823 1720655590941126800
{
  "name": "Invisibility_VFX",
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
  "aoid": "238435760880569:1720655590944709200",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/Invisibility/BAT003_invisibility.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    }
  }
},
{
  "cid": 4,
  "aoid": "238435777811573:1720655590953188200",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "FadeSprite": false,
    "PersistTime": 1,
    "FadeTime": 2
  }
},
{
  "cid": 3,
  "aoid": "238435770858311:1720655590949705900",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": false,
    "Animator": "238435760880569:1720655590944709200",
    "EntityLifeTime": 2,
    "StartAnimationStr": [
      "effect_loop"
    ]
  }
}
