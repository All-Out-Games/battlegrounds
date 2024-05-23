10
236223201281
365682631791051 1716483885753440800
{
  "name": "Portal",
  "local_enabled": true,
  "local_position": {
    "X": -6.4873809814453125,
    "Y": -5.6753573417663574
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.0799999982118607,
    "Y": 0.0800000056624413
  },
  "sibling_index": 2,
  "parent": "3995215179226:1716399837848180900",
  "spawn_as_networked_entity": true,
  "network_id": 2
},
{
  "cid": 1,
  "aoid": "365682631954311:1716483885753517900",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/portal/cosmicPortal.spine",
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
  "cid": 3,
  "aoid": "365682632059222:1716483885753567700",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "prompt_offset": {
      "X": 0,
      "Y": 1
    },
    "text": "PvPZone"
  }
},
{
  "cid": 2,
  "aoid": "366166482724117:1716484114849628600",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "SpawnPoint": "360067163054752:1716331958358429600",
    "TeleportZone": "3111059827982:1716399395061642500",
    "TeleportText": "CentralHub",
    "ChangeStatusTo": "Safe"
  }
}
