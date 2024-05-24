10
240518168577
31069919089008 1716413396906995300
{
  "name": "Portal",
  "local_enabled": true,
  "local_position": {
    "X": 1.0075421333312988,
    "Y": 0.9781417846679688
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.0500000007450581,
    "Y": 0.0500000007450581
  },
  "sibling_index": 2,
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true,
  "network_id": 3
},
{
  "cid": 1,
  "aoid": "31162563296502:1716413443303375800",
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
  "aoid": "31207523722928:1716413465819635300",
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
  "aoid": "366140754091523:1716484102667505400",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "SpawnPoint": "3995215294054:1716399837848237700",
    "TeleportZone": "3995215330322:1716399837848255900",
    "TeleportText": "PvPZone",
    "ChangeStatusTo": "Combat"
  }
}
