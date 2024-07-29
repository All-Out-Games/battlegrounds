11
210453397506
47573044970007 1720472548845770900
{
  "name": "BloodVFX",
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
  "aoid": "47573045122564:1720472548845831000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/BloodSplurt/BAT003_blood_splurt.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 2,
  "aoid": "47620533885187:1720472567871770300",
  "component_type": "Mono_Component",
  "mono_component_type": "BaseVFX",
  "data": {
    "Loop": true,
    "Animator": "47573045122564:1720472548845831000",
    "EntityLifeTime": 6,
    "StartAnimationStr": [
      "animation"
    ]
  }
},
{
  "cid": 3,
  "aoid": "47627085630115:1720472570496667000",
  "component_type": "Mono_Component",
  "mono_component_type": "AttachmentObject",
  "data": {
    "EntityLifeTime": 0
  }
},
{
  "cid": 4,
  "aoid": "47641198208735:1720472576150742600",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSpine": true,
    "FadeSprite": false,
    "PersistTime": 4,
    "FadeTime": 5
  }
}
