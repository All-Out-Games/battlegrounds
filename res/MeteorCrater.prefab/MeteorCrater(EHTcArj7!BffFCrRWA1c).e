13
322122547206
18094093416699 1720115756812209500
{
  "name": "MeteorCrater",
  "local_enabled": true,
  "local_position": {
    "X": -21.0330257415771484,
    "Y": -17.0754394531250000
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "next_sibling": "377316458197998:1716489394196426200"
},
{
  "cid": 1,
  "aoid": "18094094500410:1720115756812643300",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/meteor crater/BAT003_meteor_crash_crator.spine",
    "ordered_skins": [

    ],
    "depth_offset": 1,
    "skeleton_scale": {
      "X": 2,
      "Y": 2
    }
  }
},
{
  "cid": 2,
  "aoid": "86971272700567:1740973097392923300",
  "component_type": "Mono_Component",
  "mono_component_type": "MeteorCraterVFX",
  "data": {
    "Animator": "18094094500410:1720115756812643300",
    "EntityLifeTime": 4,
    "CraterVFX": "18094094500410:1720115756812643300",
    "ExplosionVFX": "86937056406783:1740973083684466600"
  }
}
