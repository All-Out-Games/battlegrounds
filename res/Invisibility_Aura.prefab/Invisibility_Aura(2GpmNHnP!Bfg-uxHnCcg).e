11
210453397505
237951492848079 1720655348422682400
{
  "name": "Invisibility_Aura",
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
  "aoid": "237951493912849:1720655348423215000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/Invisibility/BAT003_invisibility.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0.5000000000000000,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    }
  }
},
{
  "cid": 2,
  "aoid": "237998737837350:1720655372083031000",
  "component_type": "Mono_Component",
  "mono_component_type": "AttachmentObject",
  "data": {
    "EntityLifeTime": 5
  }
},
{
  "cid": 3,
  "aoid": "238088850422796:1720655417211524800",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": true,
    "Animator": "237951493912849:1720655348423215000",
    "EntityLifeTime": 100,
    "StartAnimationStr": [
      "effect_loop"
    ]
  }
},
{
  "cid": 4,
  "aoid": "238105349558992:1720655425474312400",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "FadeSprite": false,
    "PersistTime": 3.5000000000000000,
    "FadeTime": 5
  }
}
