11
240518168577
31069919089008 1716413396906995300
{
  "name": "Portal",
  "local_enabled": true,
  "local_position": {
    "X": -1.6919794082641602,
    "Y": -8.1192588806152344
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.3000000119209290,
    "Y": 0.3000000119209290
  },
  "previous_sibling": "10214414044086:1716402952433790600",
  "next_sibling": "36341083635957:1717793673228865000",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "31162563296502:1716413443303375800",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/CentralHub/Portals/animation/BAT003_portals.spine",
    "ordered_skins": [
      "swords"
    ],
    "depth_offset": 0,
    "skeleton_scale": {
      "X": 1,
      "Y": 1
    },
    "mask_in_shadow": false
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
    "text": "PvPZone",
    "hold_text": "",
    "radius": 2,
    "required_hold_time": 0.6000000238418579
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
