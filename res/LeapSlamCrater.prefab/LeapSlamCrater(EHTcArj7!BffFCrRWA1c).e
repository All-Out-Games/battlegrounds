11
210453397505
18094093416699 1720115756812209500
{
  "name": "LeapSlamCrater",
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
  "previous_sibling": "377316458197998:1716489394196426200"
},
{
  "cid": 1,
  "aoid": "18094094500410:1720115756812643300",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/LeapFistSlam/BAT003_leaping_fist_slam.spine",
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
  "aoid": "18247860954675:1720115818417801100",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": false,
    "Animator": "18094094500410:1720115756812643300",
    "EntityLifeTime": 3,
    "StartAnimationStr": [
      "animation"
    ]
  }
},
{
  "cid": 3,
  "aoid": "20877682244784:1720116872032228100",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "FadeSprite": false,
    "PersistTime": 2,
    "FadeTime": 2.5000000000000000
  }
}
