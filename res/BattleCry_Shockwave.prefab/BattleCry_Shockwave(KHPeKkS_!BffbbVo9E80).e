11
210453397505
44478113662142 1720214208596168500
{
  "name": "BattleCry_Shockwave",
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
  "aoid": "44478114900715:1720214208596664400",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/BattleCry/BAT003_rage_shout.spine",
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
  "cid": 2,
  "aoid": "44550723806566:1720214237686782600",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": true,
    "Animator": "0:0",
    "EntityLifeTime": 2.5000000000000000,
    "StartAnimationStr": [
      "animation"
    ]
  }
},
{
  "cid": 3,
  "aoid": "45141365503277:1720214474322172500",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "FadeSprite": false,
    "PersistTime": 1,
    "FadeTime": 2
  }
}
