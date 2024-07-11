11
210453397505
172004697650457 1720716001098440200
{
  "name": "RegenerationAura",
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
  "aoid": "172004698489763:1720716001098837000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "VFX/Regeneration/BAT003_heal_regeneration.spine",
    "ordered_skins": [

    ],
    "depth_offset": 0.5000000000000000,
    "skeleton_scale": {
      "X": 0.5000000000000000,
      "Y": 0.5000000000000000
    }
  }
},
{
  "cid": 4,
  "aoid": "174401631048021:1720717136009525700",
  "component_type": "Mono_Component",
  "mono_component_type": "RegenerationVFX",
  "data": {
    "EntityLifeTime": 6,
    "Animator": "172004698489763:1720716001098837000"
  }
}
